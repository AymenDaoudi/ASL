using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeGenerator.Roslyn
{
    public static class Consts
    {
        public static class Spaces
        {
            public static class Tabs
            {
                public static readonly SyntaxTriviaList THREE_TABS = SyntaxFactory.TriviaList(
                SyntaxFactory.ElasticTab,
                SyntaxFactory.ElasticTab,
                SyntaxFactory.ElasticTab
            );

                public static readonly SyntaxTriviaList SIX_TABS = SyntaxFactory.TriviaList(
                    SyntaxFactory.ElasticTab,
                    SyntaxFactory.ElasticTab,
                    SyntaxFactory.ElasticTab,
                    SyntaxFactory.ElasticTab,
                    SyntaxFactory.ElasticTab,
                    SyntaxFactory.ElasticTab
                );
            }

            public static class Whitespaces
            {
                public static readonly SyntaxTriviaList THREE_WHITE_SPACES = SyntaxFactory.TriviaList(
                    SyntaxFactory.ElasticWhitespace(" "),
                    SyntaxFactory.ElasticWhitespace(" "),
                    SyntaxFactory.ElasticWhitespace(" ")
                );
            }

            public static readonly SyntaxTrivia NEW_LINE = SyntaxFactory.CarriageReturnLineFeed;

        }
    }
}