using System.Collections.Generic;


    public interface IAnimated : IReferencer
    {
        void SetSequences(IReadOnlyList<M2Sequence> sequences);
    }
