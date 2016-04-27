﻿using System;
using System.Collections.Generic;
using EPiFakeMaker;
using EPiServer;
using EPiServer.ServiceLocation;
using Xunit;

namespace Marisks.SingletonPage.Tests
{
    public class Tests
    {
        [Fact]
        public void it_returns_singleton_page_by_type_on_parent_page()
        {
            var fakeRoot = FakePage.Create("Root");
            var fakeStart = FakePage.Create("Start")
                                    .ChildOf(fakeRoot)
                                    .WithChildren(RandomPages(10));
            var fakeExpected = FakePage.Create<TestPage>("Test page")
                                        .ChildOf(fakeStart);
            _fake.AddToRepository(fakeRoot);
            var expected = fakeExpected.Page;

            var actual = fakeStart.Page.GetSingletonPage<TestPage>();

            Assert.NotNull(actual);
            Assert.Equal(expected.ContentLink, actual.ContentLink);

        }

        [Fact]
        public void it_returns_singleton_page_by_parent_pages_link()
        {
            var fakeRoot = FakePage.Create("Root");
            var fakeStart = FakePage.Create("Start")
                                    .ChildOf(fakeRoot)
                                    .WithChildren(RandomPages(10));
            var fakeExpected = FakePage.Create<TestPage>("Test page")
                                        .ChildOf(fakeStart);
            _fake.AddToRepository(fakeRoot);
            var expected = fakeExpected.Page;

            var actual = fakeStart.Page.PageLink.GetSingletonPage<TestPage>();

            Assert.NotNull(actual);
            Assert.Equal(expected.ContentLink, actual.ContentLink);
        }

        [Fact]
        public void it_returns_null_when_page_not_found()
        {
            var fakeRoot = FakePage.Create("Root");
            var fakeStart = FakePage.Create("Start")
                                    .ChildOf(fakeRoot)
                                    .WithChildren(RandomPages(10));
            _fake.AddToRepository(fakeRoot);

            var actual = fakeStart.Page.GetSingletonPage<TestPage>();

            Assert.Null(actual);
        }

        [Fact]
        public void it_returns_first_page_of_type_when_multiple_pages_exists()
        {
            var fakeRoot = FakePage.Create("Root");
            var fakeStart = FakePage.Create("Start")
                                    .ChildOf(fakeRoot)
                                    .WithChildren(RandomPages(10));
            var fakeExpected = FakePage.Create<TestPage>("Test page")
                                        .ChildOf(fakeStart);
            var fakeSecond = FakePage.Create<TestPage>("Test page 2")
                                        .ChildOf(fakeStart);
            _fake.AddToRepository(fakeRoot);
            var expected = fakeExpected.Page;
            var second = fakeSecond.Page;

            var actual = fakeStart.Page.PageLink.GetSingletonPage<TestPage>();

            Assert.NotNull(actual);
            Assert.Equal(expected.ContentLink, actual.ContentLink);
            Assert.NotEqual(second.ContentLink, actual.ContentLink);
        }

        [Fact]
        public void it_adds_found_page_data_to_cache_on_first_call()
        {
            var fakeRoot = FakePage.Create("Root");
            var fakeStart = FakePage.Create("Start")
                                    .ChildOf(fakeRoot);
            var fakeExpected = FakePage.Create<TestPage>("Test page")
                                        .ChildOf(fakeStart);
            _fake.AddToRepository(fakeRoot);
            var expected = fakeExpected.Page.ContentLink;

            fakeStart.Page.PageLink.GetSingletonPage<TestPage>();

            var key = new CacheKey(typeof(TestPage), fakeStart.Page.PageLink);
            var actual = _fakeCache.InternalCache[key];
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void it_returns_singleton_page_from_second_child_level()
        {
            var fakeRoot = FakePage.Create("Root");
            var fakeStart = FakePage.Create("Start")
                                    .ChildOf(fakeRoot)
                                    .WithChildren(RandomPages(10));
            var fakeChild = FakePage.Create("Test page")
                                    .ChildOf(fakeStart)
                                    .WithChildren(RandomPages(10));
            var fakeExpected = FakePage.Create<TestPage>("Second level test page")
                                    .ChildOf(fakeChild);

            _fake.AddToRepository(fakeRoot);
            var expected = fakeExpected.Page;

            var actual = fakeStart.Page.PageLink.GetSingletonPage<TestPage>();

            Assert.NotNull(actual);
            Assert.Equal(expected.ContentLink, actual.ContentLink);
        }

        [Fact]
        public void it_returns_different_singleton_for_different_parent_pages()
        {
            var fakeRoot = FakePage.Create("Root");
            var firstStart = FakePage.Create("Start")
                                    .ChildOf(fakeRoot)
                                    .WithChildren(RandomPages(10));
            var firstSingleton = FakePage.Create<TestPage>("First test page")
                                    .ChildOf(firstStart);
            var secondStart = FakePage.Create("Second start")
                                    .ChildOf(firstStart)
                                    .WithChildren(RandomPages(10));
            var secondSingleton = FakePage.Create<TestPage>("Second test page")
                                    .ChildOf(secondStart);
            _fake.AddToRepository(fakeRoot);

            var firstActual = firstStart.Page.PageLink.GetSingletonPage<TestPage>();
            var secondActual = secondStart.Page.PageLink.GetSingletonPage<TestPage>();

            Assert.NotNull(firstActual);
            Assert.NotNull(secondActual);
            Assert.NotEqual(firstActual.ContentLink, secondActual.ContentLink);
            Assert.Equal(firstSingleton.Page.ContentLink, firstActual.ContentLink);
            Assert.Equal(secondSingleton.Page.ContentLink, secondActual.ContentLink);
        }

        //todo: implement checking for empty reference

        public IEnumerable<FakePage> RandomPages(int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return FakePage.Create(Guid.NewGuid().ToString());
            }
        }

        public Tests()
        {
            _fake = new FakeMaker();
            Extensions.InjectedContentLoader = new Injected<IContentLoader>(_fake.ContentRepository);
            _fakeCache = new FakeCache();
            Extensions.InjectedCache = new Injected<IContentReferenceCache>(_fakeCache);
        }

        private readonly FakeMaker _fake;
        private readonly FakeCache _fakeCache;
    }
}