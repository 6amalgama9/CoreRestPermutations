using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoreRestPermutations.DAL;

namespace CoreRestPermutations
{
    public class ArrayPermutations : IDisposable
    {
        private readonly List<ReturnValue> _existing;
        private List<ReturnValue> _generated;
        private PermutationsData _database;

        public ArrayPermutations()
        {
            _database = new PermutationsData(new PermutationsContext());
            _existing = GetFromDb();
            _generated = new List<ReturnValue>();
        }

        private List<ReturnValue> GetFromDb()
        {
            return _database.GetAllOriginalValues().Select(p => new ReturnValue
            {
                ValueString = p.Value,
                Seconds = p.ElapsedSeconds,
                PermutationList = _database.GetAllCombinationsByOrId(p.Id).Select(c=>c.Value).ToList()
            }).ToList();
        }

        public List<ReturnValue> GetPermutationList(List<string> originalArray)
        {
            foreach (var element in originalArray)
            {
                // if already processed (duplicated elements)
                if(_generated.Any(p => p.ValueString.Contains(element)))
                    continue;
                // check if already exist
                if (_existing.Any(p => p.ValueString.Contains(element)))
                {
                    _generated.Add(_existing.FirstOrDefault(p=> p.ValueString == element));
                    continue;
                }
                // generate permutations
                var generationResult = Generate(element);
                // save to result
                _generated.Add(generationResult);
            }

            //save new Values to DB
            _generated.Except(_existing).ToList().ForEach(p =>
            {
                var or = new OriginalValues
                {
                    Value = p.ValueString,
                    ElapsedSeconds = p.Seconds
                };
                or.Id = _database.SaveOriginalValues(or);
                p.PermutationList.ForEach(per =>
                {
                    _database.SavePermutations(new Combinations
                    {
                        Value = per,
                        OriginalValueId = or.Id
                    });
                });
            });
            return _generated;
        }

        private ReturnValue Generate(string element)
        {
            var permutations = new HashSet<string>();

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var chars = element.ToCharArray();
            
            var count = chars.Length;
            var arrResult = new char[count];
            var isInProc = new bool[count];
            Permutation(chars, 0, arrResult, isInProc, permutations);

            stopWatch.Stop();

            return new ReturnValue
            {
                ValueString = element,
                Seconds = stopWatch.Elapsed.TotalSeconds,
                PermutationList = permutations.ToList()
            };
        }

        private static void Permutation(char[] arr, int cnt, char[] arrResult, bool[] isInProc, HashSet<string> result)
        {
            if (arr.Length == cnt)
            {
                result.Add(String.Join(null, arrResult));
                return;
            }

            for (var i = 0; i < arr.Length; i++)
                if (!isInProc[i])
                {
                    isInProc[i] = true;
                    arrResult[cnt] = arr[i];
                    Permutation(arr, cnt + 1, arrResult, isInProc, result);
                    isInProc[i] = false;
                }
        }

        public void Dispose()
        {
            _database?.Dispose();
        }
    }

    public class ReturnValue
    {
        public string ValueString { get; set; }
        public double Seconds { get; set; }
        public List<string> PermutationList { get; set; }
        public int UniquePermutationCounts => PermutationList.Count;
    }
}