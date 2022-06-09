using System.Collections.Generic;

namespace GameEngine
{
    public class PoolEntry<T>
    {
        public int Id;
        public T Value;
    }

    public class PoolGeneric<T>
    {
        private readonly List<PoolEntry<T>> _availables = new List<PoolEntry<T>>();
        private readonly List<PoolEntry<T>> _inUse = new List<PoolEntry<T>>();

        public PoolEntry<T> GetorCreate(int id)
        {
            if (_availables.Count > 0)
            {
                for (var i = 0; i < _availables.Count; i++)
                {
                    if (_availables[i].Id == id)
                    {
                        var obj = _availables[i];
                        _availables.RemoveAt(i);
                        _inUse.Add(obj);
                        return obj;
                    }
                }
            }

            var newObj = new PoolEntry<T>
            {
                Id = id
            };
            
            _inUse.Add(newObj);
            return newObj;
        }

        public void InUseToAvailable(PoolEntry<T> poolEntry)
        {
            _inUse.Remove(poolEntry);
            _availables.Add(poolEntry);
        }
    }
}
