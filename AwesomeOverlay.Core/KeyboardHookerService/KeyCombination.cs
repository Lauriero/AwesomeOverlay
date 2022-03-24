
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Input;

namespace AwesomeOverlay.Core.KeyboardHookerService
{
    public class KeyCombination
    {
        private List<Key> _keys;

        public KeyCombination(params Key[] keys)
        {
            _keys = new List<Key>(keys);
        }

        public KeyCombination(IEnumerable<Key> keys)
        {
            _keys = new List<Key>(keys);
        }

        public bool IsValid()
        {
            return _keys.Count >= 2;
        }

        public override bool Equals(object obj)
        {
            KeyCombination combination2 = obj as KeyCombination;
            return Enumerable.SequenceEqual(_keys, combination2._keys);
        }
    }
}
