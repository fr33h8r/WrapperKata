using System;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;

namespace WrapperKata
{
    public class Wrapper
    {
        public string Wrap(string text, int columnWidth, bool breakAtWordBoundaries)
        {
            if (columnWidth == 0 || text.Length < columnWidth) return text;

            if (columnWidth < 20) throw new ColumnException("Re asign column width.");

            var sb = new StringBuilder(text);

            var indexOfNewLine = columnWidth;
            var spaceindex = 0;

            for (var i = 0; i < sb.Length; i++)
            {
                if (sb[i] == ' ')
                    if (i - spaceindex > columnWidth)
                        throw new ColumnException("Добро пожаловать в лес чудес.");
                    else
                        spaceindex = i;

                if (i != indexOfNewLine) continue;

                sb.Insert(breakAtWordBoundaries ? spaceindex + 1: indexOfNewLine, '\n');
                indexOfNewLine += columnWidth + (breakAtWordBoundaries ? -1 : +1);
            }
            return sb.ToString();
        }
    }

    public class ColumnException : Exception
    {
        public ColumnException(string message) : base(message) { }
    }

    public class WrapperTests
    {
        readonly Wrapper wrapper = new Wrapper();
        private const string Text = "The quick brown fox jumps over the lazy dog. The quick brown fox jumps over the lazy dog.";

        [Fact]
        public void should_return_source_string_if_width_0()
        {
            //act
            var res = wrapper.Wrap(Text, 0, false);
            //assert
            res.Should().Be(Text);
        }

        [Fact]
        public void should_wrap_strings_larger_than_23_chars()
        {
            var res = wrapper.Wrap(Text, 23, false);
            res.Should().Be("The quick brown fox jum\n" +
                            "ps over the lazy dog. T\n" +
                            "he quick brown fox jump\n" +
                            "s over the lazy dog.");
        }

        [Fact]
        public void should_break_words_at_word_boundaries()
        {
            //act
            var res = wrapper.Wrap(Text, 22, true);
            //assert
            res.Should().Be("The quick brown fox \n" +
                            "jumps over the lazy \n" +
                            "dog. The quick brown \n" +
                            "fox jumps over the \n" +
                            "lazy dog.");
        }

        [Fact]
        public void should_return_text_if_shorter_then_width()
        {
            const string expected = "The quick";
            var res = wrapper.Wrap(expected, 20, true);
            res.Should().Be(expected);
        }

        [Fact]
        public void should_throw_exception_if_column_small()
        {
            Action action = () => wrapper.Wrap(Text, 8, true);
            action.ShouldThrow<ColumnException>().WithMessage("Re asign column width.");
        }

        [Fact]
        public void should_throw_exception_if_word_longer_than_width()
        {
            Action action = () => wrapper.Wrap("sdf sdfds fhagsdfdsffjhdsgfjgdfs dsfdsf", 20, false);
            action.ShouldThrow<ColumnException>().WithMessage("Добро пожаловать в лес чудес.");
        }
    }

    static class Program
    {
        static void Main()
        {
            Console.WriteLine(new Wrapper().Wrap("The quick brown fox jumps over the lazy dog. The quick brown fox jumps over the lazy dog.", 22, true));
            Console.ReadLine();
        }
    }
}
