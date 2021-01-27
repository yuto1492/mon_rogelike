using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Container
{
    public class SubjectContainer
    {
        private List<AsyncSubject<Unit>> _subjects = new List<AsyncSubject<Unit>>();
        private int _count;
        public void Add(AsyncSubject<Unit> subject)
        {
            _subjects.Add(subject);
        }

        public AsyncSubject<Unit> Play()
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            subject.Subscribe(_ =>
            {
                AllDispose();
            });
            if (_subjects.Count != 0)
            {
                _subjects.ForEach(x =>
                {
                    x.Subscribe(_ =>
                    {
                        _count++;
                        if (_count == _subjects.Count)
                        {
                            subject.OnNext(Unit.Default);
                            subject.OnCompleted();
                        }
                    });
                });
            }
            else
            {
                Utils.ObservableUtils.AsyncSubjectTimeZeroCompleted(subject);
            }

            return subject;
        }
        
        public void AllDispose()
        {
            _subjects.ForEach(x =>
            {
                x.Dispose();
            });
            _subjects.Clear();
        }
    }
}