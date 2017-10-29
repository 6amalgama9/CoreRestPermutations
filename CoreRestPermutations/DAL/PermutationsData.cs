using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreRestPermutations.DAL
{
    public class PermutationsData : IDisposable
    {
        private readonly PermutationsContext _context;

        public PermutationsData(PermutationsContext context)
        {
            _context = context;
        }

        public List<OriginalValues> GetAllOriginalValues()
        {
            return _context.OriginalValues.ToList();
        }

        public List<Combinations> GetAllCombinationsByOrId(int id)
        {
            return _context.Combinations.Where(p=>p.OriginalValueId == id).ToList();
        }

        private OriginalValues FindValue(string val)
        {
            return _context.OriginalValues.FirstOrDefault(p => p.Value == val);
        }

        private Combinations FindCombination(string val, int originalId)
        {
            return _context.Combinations.FirstOrDefault(p => p.Value == val && p.OriginalValueId == originalId);
        }

        public int SaveOriginalValues(OriginalValues value)
        {
            var ex = FindValue(value.Value);
            if (ex == null)
            {
                _context.OriginalValues.Add(value);
            }
            else
            {
                ex.ElapsedSeconds = value.ElapsedSeconds;
            }
            _context.SaveChanges();
            return value.Id;
        }

        public int SavePermutations(Combinations value)
        {
            var ex = FindCombination(value.Value, value.OriginalValueId);
            if (ex == null)
            {
                _context.Combinations.Add(value);
            }
            else
            {
                ex.OriginalValueId = value.OriginalValueId;
            }
            _context.SaveChanges();
            return value.Id;
        }
        
        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
