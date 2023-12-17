namespace LD50.Content {
    /// <summary>
    /// A content manager that loads content from a root directory.
    /// </summary>
    public interface IContentManager {
        /// <summary>
        /// The relative path to the content manager's root directory.
        /// </summary>
        string RootDirectory { get; }

        /// <summary>
        /// Loads a content file from the content manager's root directory.
        /// </summary>
        /// <typeparam name="T">The type of content to load.</typeparam>
        /// <param name="path">The relative path to the content file.</param>
        /// <returns>The loaded content.</returns>
        T Load<T>(string path);
    }
}
