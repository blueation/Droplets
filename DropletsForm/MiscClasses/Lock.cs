using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Droplets
{
    public class TTASLock
    {
        int AtomState = 0;                                  //an indicator used to see wether or not someone allready has the lock. (0 for free, 1 for in use)

        public void LockIt()
        {
            while (true)
            {
                while (AtomState == 1) { }                           //we check wether or not the someone else is in there as long as someone is in there,
                if (Interlocked.Exchange(ref AtomState, 1) != 1)    //as soon as we don't see someone in there, we try to get the lock
                    return;
            }
        }

        public bool TryLock()
        {
            return Interlocked.Exchange(ref AtomState, 1) == 0;
            //we exchange "true" with the atomstate and check wether or not we get "true" back; if so another task is allready in there
            //and we are too late. if we get 0 back however, we have just gotten the lock ourselves.
        }

        public void UnlockIt()
        {
            AtomState = 0;
        }
    }
}
