using CommonServiceLocator;
using FluentAssertions.Common;
using ImageBrowser.Domain.SearchEngine;
using SolrNet;

namespace ImageBrowser.Application.FunctionalTests;
public class TestSearchEngine : BaseTestFixture
{
    [SetUp]  
    public void SetUpFunction()
    {
        Startup.Init<IBDataSchema>("http://localhost:8983/solr/");
    }


    [Test]
    public void Add()
    {
        var p = new IBDataSchema
        {
            Id = "SP2514N",

            Categories = new[] {
                "dog",
                "animals",
            },
            TextContent = """
INTRODUCTION

At various periods of his life Nietzsche designated different
written and unwritten books of his as his " principal work."
The composition of some of them never advanced very far, and
whilst in the midst of his " Transvaluation of all Values," the
First Part of which is the "Antichrist," he was forever disabled
by an incurable disease. If one has a right to speak of the
principal work of a mental life that never reached its goal,
but was suddenly crippled in mid-career, the strange fact ap-
pears, that Nietzsche's masterpiece is not one of his purely
philosophical books, but a work, half philosophy, half fiction ;
half an ethical sermon, half a story ; a book serio-jocular and
scientific-fantastical ; historico-satirical, and realistico-idealistic ;
a novel embracing worlds and ages and, at the same time, ex-
pressing a pure essence of Nietzsche, - his astounding prose-
poem Thus Spake Zarathustra. 
""",
            Size = 10
        };

        var solr = ServiceLocator.Current.GetInstance<ISolrOperations<IBDataSchema>>();
        solr.Add(p);
        solr.Commit();
    }


    [Test]
    public void Query()
    {
        var solr = ServiceLocator.Current.GetInstance<ISolrOperations<IBDataSchema>>();
        var results = solr.Query(new SolrQueryByField("id", "SP2514N"));
        Assert.AreEqual(1, results.Count);
        Assert.Contains("Zarathustra", results[0].TextContent.Split(" "));
        Console.WriteLine(results[0].Size);
    }



}
