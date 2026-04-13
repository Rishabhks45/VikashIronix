# Unused Items in VikashIronix Project

This document lists all unused files, classes, packages, and code items in the VikashIronix codebase.

## 📁 Unused Files & Folders

### Empty/Unused Files
1. **`src/services/VikashIronix_WebUI/Controllers/`** - Empty folder (no controllers exist)

---

## 🔧 Unused Classes & Code

### In VikashIronix_WebUI
1. **`CookieService`** (`AuthServices/CookieService.cs`)
   - Defined but **NOT registered in DI**
   - Not injected or used anywhere in the codebase
   - Note: `cookies.js` is loaded in App.razor, but CookieService class itself is unused

### In SharedKernel/DTOs
2. **`ProjectDto`** (`DTOs/ProjectDto.cs`)
   - `RequestProjectDto` - Never used
   - `ResponseProjectDto` - Never used
   - Uses `ProjectStatus` enum and `GetDisplayName()` extension, but the DTO itself is unused

3. **`GridRequestDto`** (`DTOs/GridRequestDto.cs`)
   - Defined but never referenced or used

4. **`IdResult`** (`DTOs/IdResult.cs`)
   - Defined but never used

---

## 📦 Unused NuGet Packages

### In SharedKernel
1. **`Mapster`** (v7.4.0)
   - Package is referenced but **never used**
   - No `.Adapt()` calls or Mapster usage found

### In WebApi
2. **`EPPlus`** (v8.4.0)
   - Package is referenced but **never used**
   - No `ExcelPackage` usage found

### In VikashIronix_WebUI
3. **`EPPlus`** (v8.4.0)
   - Package is referenced but **never used**
   - No `ExcelPackage` usage found

### In Directory.Packages.props (Defined but NOT referenced in any .csproj)
4. **`Blazored.LocalStorage`** (v4.5.0)
   - Defined in central package management but not referenced in any project
   - This is for client-side storage (not needed for Blazor Server)

5. **`Microsoft.AspNetCore.Components.WebAssembly.Authentication`** (v9.0.0)
   - Defined in central package management but not referenced
   - This is for WebAssembly apps, not Blazor Server

---

## 🗂️ Unused Enums (In C# Code)

In `SharedKernel/Enums/Enums.cs`, the following enums are **defined but not used in C# application code**:

**Note:** Many of these enums are referenced in database stored procedures and scripts (since VikashIronix uses the iNestHRMS database schema), but they are **not used in the C# application code** of VikashIronix. They may be needed for future features.

1. `HolidayType` - Used in DB scripts, not in C# code
2. `EmploymentStatus` - Used in DB scripts, not in C# code
3. `EmploymentType` - Used in DB scripts, not in C# code
4. `LeaveType` - Used in DB scripts, not in C# code
5. `LeaveStatus` - Used in DB scripts, not in C# code
6. `AttendanceStatus` - Not used in C# code
7. `PayrollStatus` - Not used in C# code
8. `Gender` - Used in DB scripts, not in C# code
9. `MaritalStatus` - Not used in C# code
10. `Month` - Used in DB scripts (as INT), not as enum in C# code
11. `LeaveRule` - Used in DB scripts, not in C# code
12. `MarriedStatus` - Used in DB scripts, not in C# code (duplicate concept of MaritalStatus)
13. `ProjectStatus` - Only used in unused `ProjectDto` class
14. `EmailTemplateName` - Not used in C# code
15. `Year` - Used in DB scripts (as INT), not as enum in C# code

**Used Enum:**
- `UserType` - Used in authentication flow (`AuthService.cs`, `UserContext.cs`, DTOs)

---

## 📝 Partially Used Files

### JavaScript Files
1. **`wwwroot/js/download.js`**
   - File is loaded in `App.razor`
   - `saveAsFile` function is defined but **never called** from C# code
   - Consider removing if file download functionality is not implemented

2. **`wwwroot/js/cookies.js`**
   - File is loaded in `App.razor`
   - Functions (`setCookie`, `getCookie`, `removeCookie`) are defined
   - These would be used by `CookieService`, but since `CookieService` is unused, these JS functions are also effectively unused

---

## 🎯 Summary Statistics

- **Unused Files:** 1 file/folder
- **Unused Classes:** 4 classes/DTOs
- **Unused Packages:** 5 packages
- **Unused Enums:** 15 enums (out of 16 total)
- **Partially Used:** 2 JavaScript files

---

## 💡 Recommendations

### High Priority (Safe to Remove)
1. Remove `CookieService.cs` class
2. Remove unused packages: `Mapster`, `EPPlus` (from all projects), `Blazored.LocalStorage`, `Microsoft.AspNetCore.Components.WebAssembly.Authentication`
3. Remove unused DTOs: `ProjectDto.cs`, `GridRequestDto.cs`, `IdResult.cs`
4. Remove empty `Controllers` folder (or add a `.gitkeep` if you plan to use it)

### Medium Priority (Consider Removing)
1. Remove unused JavaScript files if functionality is not needed
2. Remove unused enums (unless planning to use them soon)
3. Clean up `Directory.Packages.props` to remove unused package versions

### Low Priority (Keep if Planning to Use)
- Keep enums if you plan to implement related features
- Keep DTOs if they're part of planned features

---

## 🔍 How to Verify Usage

To verify if any item is truly unused, you can:

1. **For C# code:** Use Visual Studio's "Find All References" (Shift+F12)
2. **For packages:** Check if types from the package are imported/used
3. **For files:** Use grep/search across the codebase
4. **For JavaScript:** Check if functions are called from C# via `IJSRuntime.InvokeAsync`

---

---
*Generated for: VikashIronix Project*


