using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nerdy.Domain.Utility
{
    public class SimilarWords
    {
        public IEnumerable<string> GetWords(string text) =>
        text.Split(new[] { ' ', ',', '.', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(word => word.ToLower());
    }
}
