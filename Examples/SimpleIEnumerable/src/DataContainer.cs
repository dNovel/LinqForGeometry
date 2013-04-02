using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleIEnumerable
{
    /// <summary>
    /// Vertex like struct of integer types.
    /// </summary>
    internal struct Scontainer
    {
        internal int X;
        internal int Y;
        internal int Z;
    }

    /// <summary>
    /// Just holds some data and the user can insert more.
    /// </summary>
    class DataContainer
    {
        private List<Scontainer> _Lscontainer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="xi"></param>
        /// <param name="yi"></param>
        /// <param name="zi"></param>
        public DataContainer()
        {
            _Lscontainer = new List<Scontainer>();
        }

        /// <summary>
        /// Insert some data to the container.
        /// </summary>
        /// <param name="xi"></param>
        /// <param name="yi"></param>
        /// <param name="zi"></param>
        public void InsertData(int xi, int yi, int zi)
        {
            _Lscontainer.Add(
                    new Scontainer()
                    {
                        X = xi,
                        Y = yi,
                        Z = zi
                    }
                );
        }

        /// <summary>
        /// Returns an enumerable collection for every scontainer in the list where x (the first element of the struct) is an even number.
        /// </summary>
        /// <returns>IEnumerable of type sContainer</returns>
        public IEnumerable<Scontainer> EnXEvenNumber() {
            IEnumerable<Scontainer> res = from value in _Lscontainer.Where(value => value.X % 2 == 0) select value;
            return res;
        }
    }
}
