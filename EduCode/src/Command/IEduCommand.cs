﻿using EduCode.Board;

namespace EduCode.Command;

public interface IEduCommand
{
    public void Execute(EduBoard board);
    public int MaximumDepth { get; }
}