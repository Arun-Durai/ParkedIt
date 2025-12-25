# Git Ignore Guide - Files to Exclude from GitHub

This document explains which files and directories should **NOT** be committed to Git and why.

## 🚫 Files You Should NEVER Commit

### 1. Build Output Directories

#### `bin/` and `obj/`
**Why exclude:**
- **Generated files**: These are automatically created when you build the project
- **Large files**: Compiled DLLs, executables, and debug symbols can be large
- **Platform-specific**: Build output varies by OS and architecture
- **Regenerated**: Can be recreated anytime with `dotnet build`
- **Clutters repository**: Makes it harder to see actual code changes

**What's in there:**
- `ParkedIt.dll` - Compiled assembly
- `ParkedIt.exe` - Executable (on Windows)
- `ParkedIt.pdb` - Debug symbols
- `*.deps.json` - Dependency information
- `*.runtimeconfig.json` - Runtime configuration

**Example:**
```
bin/Debug/net10.0/ParkedIt.dll  ❌ Don't commit
bin/Debug/net10.0/ParkedIt.pdb  ❌ Don't commit
```

---

### 2. Intermediate Build Files

#### `obj/` directory
**Why exclude:**
- **Temporary files**: Intermediate compilation artifacts
- **Cache files**: Build cache that speeds up compilation
- **Generated code**: Auto-generated files like `*.GlobalUsings.g.cs`
- **Platform-specific**: Different on each machine
- **Regenerated**: Recreated on every build

**What's in there:**
- `*.csproj.CoreCompileInputs.cache` - Build cache
- `*.GlobalUsings.g.cs` - Generated using statements
- `*.AssemblyInfo.cs` - Generated assembly info
- `*.assets.cache` - Asset cache

**Example:**
```
obj/Debug/net10.0/ParkedIt.GlobalUsings.g.cs  ❌ Don't commit
obj/Debug/net10.0/*.cache                      ❌ Don't commit
```

---

### 3. Operating System Files

#### macOS: `.DS_Store`
**Why exclude:**
- **System file**: Created by macOS Finder
- **Not needed**: Not used by your application
- **User-specific**: Different on each machine
- **Clutters repository**: No value to other developers

**Example:**
```
.DS_Store           ❌ Don't commit
.AppleDouble        ❌ Don't commit
._*                 ❌ Don't commit (resource forks)
```

#### Windows: `Thumbs.db`, `Desktop.ini`
**Why exclude:**
- **System files**: Created by Windows Explorer
- **Not needed**: Not used by your application
- **User-specific**: Different on each machine

---

### 4. IDE/Editor Files

#### Visual Studio: `.vs/`, `*.suo`, `*.user`
**Why exclude:**
- **User preferences**: Window positions, breakpoints, etc.
- **Machine-specific**: Different on each developer's machine
- **Large files**: Can be several MB
- **Not needed**: Other developers have their own settings

**Example:**
```
.vs/                ❌ Don't commit (entire directory)
*.suo               ❌ Don't commit (user options)
*.user              ❌ Don't commit (user-specific)
```

#### VS Code: `.vscode/` (selective)
**Why exclude (selectively):**
- **User settings**: Personal preferences
- **Workspace settings**: Can be shared (optional)
- **Extensions**: User-specific extensions list

**What to exclude:**
```
.vscode/settings.json     ❌ Usually exclude (user preferences)
.vscode/launch.json       ✅ Can commit (shared debug config)
.vscode/tasks.json        ✅ Can commit (shared build tasks)
.vscode/extensions.json   ❌ Exclude (user-specific extensions)
```

#### JetBrains Rider: `.idea/`
**Why exclude:**
- **User preferences**: Personal IDE settings
- **Large directory**: Can contain many files
- **Machine-specific**: Different on each machine

---

### 5. NuGet Package Files

#### `packages/`, `*.nupkg`, `project.lock.json`
**Why exclude:**
- **Large files**: NuGet packages can be large
- **Regenerated**: Restored via `dotnet restore`
- **Version-specific**: Different package versions on different machines
- **Already tracked**: Package references in `.csproj` are enough

**What's excluded:**
```
packages/                  ❌ Don't commit
*.nupkg                    ❌ Don't commit
*.snupkg                   ❌ Don't commit
project.lock.json          ❌ Don't commit (auto-generated)
project.assets.json        ❌ Don't commit (in obj/)
```

**Note**: The `.csproj` file **SHOULD** be committed (it lists package references)

---

### 6. Test Results and Coverage

#### `TestResults/`, `*.trx`, `*.coverage`
**Why exclude:**
- **Generated**: Created when running tests
- **Large files**: Test results can be large
- **Temporary**: Not needed long-term
- **Regenerated**: Can be recreated by running tests

---

### 7. Temporary and Backup Files

#### `*.tmp`, `*.temp`, `*.bak`, `*.swp`
**Why exclude:**
- **Temporary**: Created by editors/IDEs
- **Not needed**: Not part of the actual codebase
- **Clutters repository**: No value to other developers

---

## ✅ Files You SHOULD Commit

### Source Code
```
✅ *.cs                    - C# source files
✅ *.csproj               - Project file (includes package references)
✅ *.sln                   - Solution file (if you have one)
```

### Configuration
```
✅ Config/parkingConfig.json  - Your application configuration
✅ .gitignore              - Git ignore rules (this file!)
✅ README.md               - Documentation
```

### Documentation
```
✅ README.md
✅ ARCHITECTURE.md
✅ CODE_WALKTHROUGH.md
✅ QUICK_START.md
```

---

## 📋 Quick Reference Table

| File/Directory | Exclude? | Reason |
|---------------|----------|--------|
| `bin/` | ✅ Yes | Build output, regenerated |
| `obj/` | ✅ Yes | Intermediate files, regenerated |
| `.DS_Store` | ✅ Yes | macOS system file |
| `.vs/` | ✅ Yes | Visual Studio user settings |
| `*.suo` | ✅ Yes | Visual Studio user options |
| `*.user` | ✅ Yes | User-specific settings |
| `.vscode/settings.json` | ✅ Yes | VS Code user preferences |
| `.vscode/launch.json` | ⚠️ Optional | Can share debug config |
| `packages/` | ✅ Yes | NuGet packages, large |
| `*.nupkg` | ✅ Yes | NuGet package files |
| `project.lock.json` | ✅ Yes | Auto-generated |
| `*.cs` | ❌ No | Source code, must commit |
| `*.csproj` | ❌ No | Project file, must commit |
| `Config/*.json` | ❌ No | Application config, must commit |
| `README.md` | ❌ No | Documentation, must commit |

---

## 🔍 How to Check What's Being Tracked

### See what Git is tracking:
```bash
git ls-files
```

### See what's ignored:
```bash
git status --ignored
```

### Check if a file is ignored:
```bash
git check-ignore -v path/to/file
```

---

## 🛠️ If You Accidentally Committed These Files

### Remove from Git (but keep locally):
```bash
# Remove bin/ and obj/ from Git tracking
git rm -r --cached bin/ obj/

# Remove .DS_Store
git rm --cached .DS_Store

# Commit the removal
git commit -m "Remove build artifacts and system files"
```

### Remove from Git history (if already pushed):
```bash
# Use git filter-branch or BFG Repo-Cleaner
# This is more complex - research before doing this
```

---

## 📝 Best Practices

1. **Create `.gitignore` early**: Before first commit
2. **Use standard templates**: .NET has standard `.gitignore` patterns
3. **Review before commit**: Use `git status` to see what will be committed
4. **Keep it updated**: Add new patterns as needed
5. **Document exceptions**: If you need to commit something unusual, document why

---

## 🎯 For This Project Specifically

### Already in `.gitignore`:
- ✅ `bin/` - Build output
- ✅ `obj/` - Intermediate files
- ✅ `.DS_Store` - macOS files
- ✅ `.vs/` - Visual Studio files
- ✅ `*.user`, `*.suo` - User-specific files
- ✅ `packages/` - NuGet packages
- ✅ Test results and coverage files

### Should be committed:
- ✅ All `.cs` source files
- ✅ `ParkedIt.csproj` - Project file
- ✅ `Config/parkingConfig.json` - Configuration
- ✅ All documentation (`.md` files)
- ✅ `.gitignore` - This file itself

---

## 💡 Pro Tips

1. **Use `git status` before committing**: Always check what will be committed
2. **Use `git add .` carefully**: It adds everything, including files you might not want
3. **Use `git add -p`**: Interactive staging lets you review each change
4. **Keep `.gitignore` in repository**: So all team members use same rules
5. **Review periodically**: As project grows, update `.gitignore` as needed

---

**Remember**: When in doubt, if a file is:
- **Generated automatically** → Exclude it
- **User-specific** → Exclude it
- **Large and regenerable** → Exclude it
- **Source code or config** → Commit it

