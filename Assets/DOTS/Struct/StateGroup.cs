using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace DOTS.Struct
{
    public struct StateGroup : IDisposable, IEnumerable<State>
    {
        [NativeDisableParallelForRestriction]
        private NativeList<State> _states;

        public StateGroup(int initialCapacity, Allocator allocator)
        {
            _states = new NativeList<State>(
                initialCapacity, allocator);
        }

        public StateGroup(StateGroup copyFrom, Allocator allocator)
        {
            _states = new NativeList<State>(copyFrom.Length(), allocator);
            for (var i = 0; i < copyFrom._states.Length; i++)
            {
                var state = copyFrom._states[i];
                _states.Add(state);
            }
        }

        public StateGroup(int length, NativeMultiHashMap<Node, State>.Enumerator copyFrom,
            Allocator allocator)
        {
            _states = new NativeList<State>(length, allocator);
            foreach (var state in copyFrom)
            {
                _states.Add(state);
            }
        }
        
        public StateGroup(ref DynamicBuffer<State> statesBuffer,
            Allocator allocator)
        {
            _states = new NativeList<State>(statesBuffer.Length, allocator);
            foreach (var state in statesBuffer)
            {
                _states.Add(state);
            }
        }

        public State this[int key]
        {
            get => _states[key];
            set => _states[key] = value;
        }

        public int Length()
        {
            return _states.Length;
        }

        public void Dispose()
        {
            _states.Dispose();
        }

        public void Add(State state)
        {
            _states.Add(state);
        }

        /// <summary>
        /// Fit项则无视，不同项则增加
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public void Merge(StateGroup other)
        {
            //todo 还需要考虑冲突可能，即针对同一个目标的两个state不相容
            for (var i = 0; i < other._states.Length; i++)
            {
                var otherState = other._states[i];
                var contained = false;
                for (var j = 0; j < _states.Length; j++)
                {
                    if (_states[j].Fits(otherState))
                    {
                        contained = true;
                        break;
                    }
                }

                if (!contained) _states.Add(otherState);
            }
        }

        /// <summary>
        /// Fit项则移除，不同项则无视
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public void Sub(StateGroup other)
        {
            foreach (var otherState in other._states)
            {
                for (var i = _states.Length - 1; i >= 0; i--)
                {
                    var state = _states[i];
                    if (state.Fits(otherState))
                    {
                        _states.RemoveAtSwapBack(i);
                    }
                }
            }
        }

        public State GetState(Func<State, bool> compare)
        {
            foreach (var state in _states)
            {
                if (compare(state)) return state;
            }
            return State.Null;
        }

        public State GetBelongingState(State belongTo)
        {
            foreach (var state in _states)
            {
                if (state.BelongTo(belongTo)) return state;
            }
            return State.Null;
        }

        public void WriteBuffer(ref DynamicBuffer<State> buffer)
        {
            foreach (var state in _states)
            {
                buffer.Add(state);
            }
        }

        public IEnumerator<State> GetEnumerator()
        {
            return _states.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override int GetHashCode()
        {
            var sum = 0;
            foreach (var state in _states)
            {
                sum += state.GetHashCode();
            }

            return sum;
        }
    }
}