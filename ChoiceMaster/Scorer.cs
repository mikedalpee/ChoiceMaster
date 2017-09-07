using PublishSubscribe.IntraProcessPublishSubscribe;
using System;

namespace ChoiceMaster
{
    public abstract class Scorer : Publisher<string>,IDisposable
    {
        public class RatingChange : Topic<string>
        {
            public const string TopicIdentifier = nameof(Scorer)+"."+nameof(RatingChange);
            public float NewRate
            {
                get;
                private set;
            }
            public RatingChange(double newRate) : base(TopicIdentifier)
            {
                NewRate = NewRate;
            }
        }
        private double rating = 0.0;
        public double Rating
        {
            get
            {
                return rating;
            }
            protected set
            {
                double newRating;

                if (value > 1.0)
                {
                    newRating = 1.0;
                }
                else if (value < 0.0)
                {
                    newRating = 0.0;
                }
                else
                {
                    newRating = value;
                }

                if (newRating != rating)
                {
                    rating = value;

                    MyBroker.Instance.Publish(this,new RatingChange(rating));
                }
            }
        }
        public abstract ScorerType ScorerType { get; protected set; }
        public Scorer() : base("Scorer")
        {
            MyBroker.Instance.Register(this, Scorer.RatingChange.TopicIdentifier);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    MyBroker.Instance.Unregister(this, Scorer.RatingChange.TopicIdentifier);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Scorer() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
