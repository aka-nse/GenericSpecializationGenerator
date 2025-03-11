using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace GenericSpecializationGenerator;

[InterpolatedStringHandler]
public class SourceCodeGenerationHandler(int literalLength, int formattedCount)
{
    private class StringSegment(ReadOnlyMemory<char> value, StringSegment? next = null)
    {
        public ReadOnlyMemory<char> Value { get; } = value;
        public StringSegment? Next { get; } = next;

        public StringSegment With(StringSegment next)
            => Next is null
            ? new(Value, next)
            : new(Value, Next.With(next));

        public override string ToString()
            => Value.ToString() + Next?.ToString();

        public static implicit operator StringSegment(ReadOnlyMemory<char> value)
            => new(value);
    }

    private readonly List<StringSegment> _lines = [ReadOnlyMemory<char>.Empty];

    public void AppendLiteral(string s)
    {
        var enumerator = new LineBreakSplitter(s).GetEnumerator();
        if(!enumerator.MoveNext())
        {
            return;
        }
        _lines[_lines.Count - 1] = _lines[_lines.Count - 1].With(enumerator.Current);
        while(enumerator.MoveNext())
        {
            _lines.Add(enumerator.Current);
        }
    }

    public void AppendFormatted(IndentedForeachContainer foreachStream)
    {
        StringSegment indent;
        if(_lines.Any())
        {
            indent = _lines[_lines.Count - 1];
            _lines.RemoveAt(_lines.Count - 1);
        }
        else
        {
            indent = ReadOnlyMemory<char>.Empty;
        }
        foreach (var item in foreachStream.Items)
        {
            foreach(var line in new LineBreakSplitter(item))
            {
                _lines.Add(indent.With(line));
            }
        }
    }

    public void AppendFormatted(SourceCodeGenerationHandler recursive)
    {
        StringSegment indent;
        if (_lines.Any())
        {
            indent = _lines[_lines.Count - 1];
            _lines.RemoveAt(_lines.Count - 1);
        }
        else
        {
            indent = ReadOnlyMemory<char>.Empty;
        }
        foreach (var line in recursive._lines)
        {
            _lines.Add(indent.With(line));
        }
    }

    public void AppendFormatted(IEnumerable<SourceCodeGenerationHandler> recursive)
    {
        StringSegment indent;
        if (_lines.Any())
        {
            indent = _lines[_lines.Count - 1];
            _lines.RemoveAt(_lines.Count - 1);
        }
        else
        {
            indent = ReadOnlyMemory<char>.Empty;
        }
        foreach (var recursiveHandler in recursive)
        {
            foreach (var line in recursiveHandler._lines)
            {
                _lines.Add(indent.With(line));
            }
        }
    }

    public void AppendFormatted(object obj)
    {
        if (!_lines.Any())
        {
            _lines.Add(ReadOnlyMemory<char>.Empty);
        }
        _lines[_lines.Count - 1] = _lines[_lines.Count - 1].With(obj.ToString().AsMemory());
    }

    public override unsafe string ToString()
    {
        var sb = new StringBuilder(literalLength + formattedCount * 16);
        foreach (var line in _lines)
        {
            var current = line;
            while (current != null)
            {
                fixed(char* ptr = current.Value.Span)
                {
                    sb.Append(ptr, current.Value.Length);
                }
                current = current.Next;
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}

public static class SourceCodeGenerationHandlerHelpers
{
    public static IndentedForeachContainer ForeachIndented(this IEnumerable<string> items)
        => new(items);

    public static IndentedForeachContainer ForeachIndented<T>(this IEnumerable<T> items, Func<T, string> predicate)
        => new(items.Select(predicate));
}

public readonly ref struct IndentedForeachContainer(IEnumerable<string> items)
{
    public readonly IEnumerable<string> Items { get; } = items;
}


file ref struct LineBreakSplitter(string s)
{
    public ref struct Enumerator(string s)
    {
        private int _nextStart = 0;
        private int _start = 0;
        private int _end = 0;

        public readonly ReadOnlyMemory<char> Current
            => s.AsMemory(_start, _end - _start);

        public bool MoveNext()
        {
            if(_nextStart > s.Length)
            {
                return false;
            }
            if(_nextStart == s.Length)
            {
                _start = _nextStart;
                _end = _nextStart;
                ++_nextStart;
                return true;
            }
            _start = _nextStart;
            _end = _nextStart;
            while(_end < s.Length)
            {
                if(s.AsSpan(_end).StartsWith(['\r', '\n'], StringComparison.Ordinal))
                {
                    _nextStart = _end + 2;
                    return true;
                }
                if (s[_end] == '\r' || s[_end] == '\n')
                {
                    _nextStart = _end + 1;
                    return true;
                }
                ++_end;
            }
            _nextStart = _end + 1;
            return true;
        }
    }

    public readonly Enumerator GetEnumerator() => new (s);
}
