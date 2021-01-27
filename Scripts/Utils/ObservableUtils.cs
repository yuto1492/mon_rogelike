using System;
using UniRx;

namespace Utils
{
    public class ObservableUtils
    {
        public static void AsyncSubjectTimeZeroCompleted(AsyncSubject<Unit> subject)
        {
            Observable.Timer(TimeSpan.FromMilliseconds(0)).Subscribe(_ =>
            {
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            });
        }

        public static AsyncSubject<Unit> Timer(float frame)
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            Observable.Timer(TimeSpan.FromMilliseconds(frame)).Subscribe(_ =>
            {
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            });
            return subject;
        }
        
    }
}