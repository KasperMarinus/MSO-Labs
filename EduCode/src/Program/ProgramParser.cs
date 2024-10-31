﻿using System.IO;
using EduCode.Command;

namespace EduCode.Program;

public class ProgramParser
{
    private readonly string[] _lines;
    private int _currentLineIndex = 0;
    private readonly Stack<int> _indentStack = new();

    public ProgramParser(string path)
    {
        _lines = File.ReadAllLines(path);
        _indentStack.Push(0);
    }

    public EduProgram Parse()
    {
        var commands = ParseBlock();
        return new EduProgram(commands);
    }

    private List<IEduCommand> ParseBlock()
    {
        List<IEduCommand> commands = new();

        while (_currentLineIndex < _lines.Length)
        {
            var line = _lines[_currentLineIndex];

            if (string.IsNullOrWhiteSpace(line))
            {
                _currentLineIndex++;
                continue;
            }

            var actualIndent = GetIndentation(line);
            var expectedIndent = _indentStack.Peek();

            if (actualIndent < expectedIndent)
            {
                _indentStack.Pop();
                break;
            }
            if (actualIndent > expectedIndent)
            {
                throw new FormatException("Invalid indentation");
            }

            commands.Add(ParseCommand(line));
            _currentLineIndex++;
        }

        return commands;
    }

    private IEduCommand ParseCommand(string line)
    {
        var trimmedLine = line.Trim();
        var parts = trimmedLine.Split(' ');

        return parts[0] switch
        {
            "Move" => ParseMove(parts),
            "Turn" => ParseTurn(parts),
            "Repeat" => ParseRepeat(parts),
            _ => throw new FormatException("Invalid command")
        };
    }

    private IEduCommand ParseMove(string[] parts)
    {
        if (parts.Length != 2)
        {
            throw new FormatException("Invalid move command");
        }

        return new MoveCommand(int.Parse(parts[1]));
    }

    private IEduCommand ParseTurn(string[] parts)
    {
        if (parts.Length != 2)
        {
            throw new FormatException("Invalid turn command");
        }

        return new TurnCommand(parts[1]);
    }

    private IEduCommand ParseRepeat(string[] parts)
    {
        if (parts.Length != 2)
        {
            throw new FormatException("Invalid repeat command");
        }

        _indentStack.Push(GetIndentation(_lines[_currentLineIndex + 1]));
        _currentLineIndex++;

        var count = int.Parse(parts[1]);
        var block = ParseBlock();
        return new RepeatCommand(count, block);
    }

    private static int GetIndentation(string line)
    {
        var indent = 0;
        foreach (var c in line)
        {
            if (c == ' ')
            {
                indent++;
            }
            else
            {
                break;
            }
        }

        return indent;
    }
}