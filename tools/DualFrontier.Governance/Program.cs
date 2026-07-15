using DualFrontier.Governance;

// DualFrontier.Governance entry point. All logic lives in the pure/static
// classes (RepoPaths, CorpusWalker, FrontmatterExtractor, ExclusionConfig, and
// -- added next in the cascade -- Validators) so the same code is consumed by
// this CLI and by the xUnit suite.
return GovernanceCli.Run(args);
