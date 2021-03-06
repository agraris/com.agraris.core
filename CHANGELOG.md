# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

### Guiding Principles
- Changelogs are for humans, not machines.
- There should be an entry for every single version.
- The same types of changes should be grouped.
- Versions and sections should be linkable.
- The latest version comes first.
- The release date of each version is displayed.
- Mention whether you follow Semantic Versioning.

### Types of changes
- **Added** for new features.
- **Changed** for changes in existing functionality.
- **Deprecated** for soon-to-be removed features.
- **Removed** for now removed features.
- **Fixed** for any bug fixes.
- **Security** in case of vulnerabilities.

## [Unreleased]
### Added
- GameData.cs for data saving
- Singleton.cs and PresistentSingleton.cs for instancing
- FloatingMonoBehaviour.cs for calling Coroutine without attaching the script on gameObject
- AndroidBuildPipeline.cs helper for Android build
- SimplePool.cs for object pooling by quill18 and Draugor
- Versioning.cs for incrementing version and bundleVersionCode (Android) with single click
- AndroidKeystoreAuthenticatorOSX.cs fill keystore password on build automatically for OSX by Adrian Stutz
- AndoridKeystoreAuthenticatorWindows.cs fill keystore password on build automatically for Windows
