# RandomSkunk.Logging.Testing.Moq

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog],
and this project adheres to [Semantic Versioning].

## [Unreleased]

### Added

- Create initial project, solution, and package structures.
- Add `TestingLogger`, an implementation of `ILogger` for testing.
- Add `TestingLogger<TCategoryName>`, an implementation of `ILogger<TCategory>` for testing.
- Add `MockLogger`, an inheritor of `Mock<TestingLogger>`.
- Add `VerifyLog` & `SetupLog` extension methods.

[Keep a Changelog]: https://keepachangelog.com/en/1.0.0/
[Semantic Versioning]: https://semver.org/spec/v2.0.0.html
[Unreleased]: https://github.com/bfriesen/RandomSkunk.Logging.Testing.Moq/compare/b77fae71a17716c64e37cce545223d3027f049e8...HEAD
