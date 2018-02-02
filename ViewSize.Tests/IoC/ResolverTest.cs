// <copyright file="ResolverTest.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using CRLFLabs.ViewSize.IoC;
using FluentAssertions;
using NUnit.Framework;

namespace ViewSize.Tests.IoC
{
    public class ResolverTest
    {
        public class NoDependencies
        {
            [Test]
            public void NoArgsConstructor()
            {
                // arrange
                Resolver resolver = new Resolver();

                // act
                var result = resolver.Resolve<Simple>();

                // assert
                Assert.IsNotNull(result);
            }

            [Test]
            public void ResolveTwiceShouldCreateNewObject()
            {
                // arrange
                Resolver resolver = new Resolver();

                // act
                var a = resolver.Resolve<Simple>();
                var b = resolver.Resolve<Simple>();

                // assert
                Assert.IsNotNull(a);
                Assert.IsNotNull(b);
                Assert.AreNotSame(a, b);
            }
        }

        public class ResolveInterface
        {
            [Test]
            public void ExplicitMapping()
            {
                // arrange
                Resolver resolver = new Resolver();
                resolver.Map<ISimple, Simple>();

                // act
                var result = resolver.Resolve<ISimple>();

                // assert
                result.Should().BeOfType<Simple>();
            }

            [Test]
            public void Convention()
            {
                // arrange
                Resolver resolver = new Resolver();

                // act
                var result = resolver.Resolve<ISimple>();

                // assert
                result.Should().BeOfType<Simple>();
            }
        }

        public class LazyInterface
        {
            [Test]
            public void Convention()
            {
                // arrange
                Resolver resolver = new Resolver();

                // act
                var result = resolver.Resolve<Lazy<ISimple>>();

                // assert
                result.Should().BeOfType<Lazy<ISimple>>();
                result.Value.Should().BeOfType<Simple>();
            }
        }

        public class Singletons
        {
            [Test]
            public void Resolve_SingletonOnce_ShouldWork()
            {
                // arrange
                Resolver resolver = new Resolver();
                resolver.Map<ISimple, Simple>(true);

                // act
                var result = resolver.Resolve<ISimple>();

                // assert
                Assert.IsNotNull(result);
            }

            [Test]
            public void Resolve_SingletonTwice_ShouldResolveSameObject()
            {
                // arrange
                Resolver resolver = new Resolver();
                resolver.Map<ISimple, Simple>(true);

                // act
                var a = resolver.Resolve<ISimple>();
                var b = resolver.Resolve<ISimple>();

                // assert
                Assert.IsNotNull(a);
                Assert.IsNotNull(b);
                Assert.AreSame(a, b);
            }
        }

        public class Dependencies
        {
            [Test]
            public void Resolve_ArgsConstructorWithClassArgument_ShouldResolveDependencies()
            {
                // arrange
                Resolver resolver = new Resolver();

                // act
                var result = resolver.Resolve<OwnsSimple>();

                // assert
                Assert.IsNotNull(result);
                Assert.IsNotNull(result.Dependency);
            }

            [Test]
            public void Resolve_ArgsConstructorWithInterfaceArgument_ShouldResolveDependencies()
            {
                // arrange
                Resolver resolver = new Resolver();
                resolver.Map<ISimple, Simple>();

                // act
                var result = resolver.Resolve<OwnsISimple>();

                // assert
                Assert.IsNotNull(result);
                Assert.IsNotNull(result.Dependency);
            }
        }

        public class ExistingInstance
        {
            [Test]
            public void Resolve_WithExistingInstance_ShouldWork()
            {
                // arrange
                Resolver resolver = new Resolver();
                var simple = new Simple();
                resolver.MapExistingInstance(typeof(ISimple), simple);

                // act
                var result = resolver.Resolve<ISimple>();

                // assert
                Assert.AreSame(simple, result);
            }

            [Test]
            public void Map_WithWrongInstanceType_Throws()
            {
                // arrange
                Resolver resolver = new Resolver();
                var ownsISimple = new OwnsISimple(new Simple());

                // act & assert
                Assert.Throws<ArgumentException>(() =>
                {
                    resolver.MapExistingInstance(typeof(ISimple), ownsISimple);
                });
            }
        }
    }
}
