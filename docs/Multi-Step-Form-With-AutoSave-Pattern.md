# Multi-Step Form with Auto-Save Pattern

## Overview
This document explains the implementation pattern for creating multi-step forms with automatic data saving at each step. This pattern is demonstrated in the Referral Source management feature.

---

## Architecture Pattern

### Core Concept
- **Save data at each step** before moving to the next
- Capture generated IDs and pass them to subsequent steps
- Allow navigation back to edit previous steps
- Final step is review and confirmation (no additional save needed)

---

## Implementation Structure

### 1. **Page Structure**

```
Step 1: Organization → Save → Get ID
Step 2: Location (requires Org ID) → Save → Get ID  
Step 3: Contact (requires Org ID) → Save → Get ID
Step 4: Review → Submit (no save, just redirect)
```

### 2. **State Management**

```csharp
// Separate models for each step
private UpsertReferralInfoRequestDto referralRequest = new();
private UpsertReferralLocationRequestDto locationRequest = new();
private UpsertReferralContactRequestDto contactRequest = new();

// Track IDs generated at each step
private Guid? referralId;
private Guid? locationId;
private Guid? contactId;

// Current step tracking
private int Step = 1;

// Loading state
private bool isSubmitting = false;
```

---

## Step-by-Step Implementation

### Step 1: Organization Details

**Form Structure:**
```razor
<EditForm Model="@referralRequest" OnValidSubmit="() => SaveReferralInfoAndNext()" novalidate>
    <DataAnnotationsValidator />
    <MudGrid>
        <!-- Form fields here -->
    </MudGrid>
    <MudButton ButtonType="ButtonType.Submit" Disabled="@isSubmitting">
        @if (isSubmitting)
        {
            <MudProgressCircular />
            <span>Saving...</span>
        }
        else
        {
            <span>Next</span>
        }
    </MudButton>
</EditForm>
```

**Save Logic:**
```csharp
private async Task SaveReferralInfoAndNext()
{
    isSubmitting = true;
    
    // Call API to save organization
    var referralResult = await masterService.UpsertReferralInfoAsync(referralRequest);
    
    if (!referralResult.Success)
    {
        isSubmitting = false;
        ShowErrorModal(referralResult.Errors);
        return;
    }
    
    // 🔑 Capture the generated ID
    referralId = referralResult.Data;
    
    // 🔑 Set the foreign key for next step
    locationRequest.ReferralInfoId = referralId.Value;
    contactRequest.ReferralInfoId = referralId.Value;
    
    isSubmitting = false;
    Snackbar.Add("Organization details saved successfully!", Severity.Success);
    
    await Task.Delay(50); // Small delay for UI feedback
    SwitchTab(2); // Move to next step
}
```

---

### Step 2: Location Details

**Save Logic:**
```csharp
private async Task SaveLocationAndNext()
{
    isSubmitting = true;
    
    // locationRequest.ReferralInfoId already set in Step 1
    var locationResult = await masterService.UpsertReferralLocationAsync(locationRequest);
    
    if (!locationResult.Success)
    {
        isSubmitting = false;
        ShowErrorModal(locationResult.Errors);
        return;
    }
    
    // 🔑 Capture location ID
    locationId = locationResult.Data;
    
    isSubmitting = false;
    Snackbar.Add("Location details saved successfully!", Severity.Success);
    
    await Task.Delay(50);
    SwitchTab(3);
}
```

**Navigation Buttons:**
```razor
<MudStack Style="display:flex; flex-direction:row !important; justify-content:space-between;">
    <MudButton Variant="Variant.Outlined" OnClick="() => GoToStep(1)">
        Previous
    </MudButton>
    <MudButton ButtonType="ButtonType.Submit" Disabled="@isSubmitting">
        Next
    </MudButton>
</MudStack>
```

---

### Step 3: Contact Details

**Save Logic:**
```csharp
private async Task SaveContactAndNext()
{
    isSubmitting = true;
    StateHasChanged();
    
    // contactRequest.ReferralInfoId already set in Step 1
    var contactResult = await masterService.UpsertReferralContactAsync(contactRequest);
    
    if (!contactResult.Success)
    {
        isSubmitting = false;
        ShowErrorModal(contactResult.Errors);
        return;
    }
    
    // 🔑 Capture contact ID
    contactId = contactResult.Data;
    
    isSubmitting = false;
    Snackbar.Add("Contact details saved successfully!", Severity.Success);
    
    await Task.Delay(50);
    SwitchTab(4); // Go to review
}
```

---

### Step 4: Review & Submit

**Purpose:** Display all saved data for final review

**Key Points:**
- No actual save operation needed (data already saved)
- Just show success message and redirect
- All data is already in the database

```csharp
private async Task SubmitReferralSource()
{
    isSubmitting = true;
    
    // No API call needed - everything already saved!
    Snackbar.Add("Referral source created successfully!", Severity.Success);
    
    isSubmitting = false;
    NavigationManager.NavigateTo("/referral");
}
```

**Review Display:**
```razor
<MudPaper Elevation="1" Class="pa-4 mb-4">
    <MudText Typo="Typo.h6" Class="mb-3">Organization</MudText>
    <MudGrid>
        <MudItem xs="12" md="6">
            <MudText>Name: @referralRequest.OrganizationName</MudText>
            <MudText>Contact: @referralRequest.FirstName @referralRequest.LastName</MudText>
        </MudItem>
        <!-- More fields -->
    </MudGrid>
</MudPaper>
```

---

## Custom Stepper UI

### Visual Stepper Component
```razor
<div class="custom-stepper">
    @for (int i = 1; i <= 4; i++)
    {
        var stepName = i switch
        {
            1 => "Organization",
            2 => "Location",
            3 => "Contact",
            _ => "Review"
        };
        
        <div class="step-wrapper">
            <div class="step-circle @(Step == i ? "active" : Step > i ? "completed" : "")">
                @i
            </div>
            <span class="step-label @(Step == i ? "active-text" : Step > i ? "completed-text" : "")">
                @stepName
            </span>
        </div>
        
        @if (i < 4)
        {
            <div class="step-line @(Step > i ? "completed" : "")"></div>
        }
    }
</div>
```

### CSS Classes
- `.step-circle.active` - Current step (purple gradient)
- `.step-circle.completed` - Completed step (green)
- `.step-line.completed` - Connector between completed steps

---

## Data Transfer Objects (DTOs)

### Step 1: Organization DTO
```csharp
public class UpsertReferralInfoRequestDto
{
    public Guid ReferralInfoId { get; set; }
    
    [Required(ErrorMessage = "Please enter the organization name.")]
    public string OrganizationName { get; set; }
    
    [Required, Range(1, int.MaxValue)]
    public int? BussinessType { get; set; }
    
    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }
    
    [Required, EmailAddress]
    public string? Email { get; set; }
    
    [Required]
    public string? Phone { get; set; }
    
    public string? PhoneCountryCode { get; set; }
    public string? EinTaxId { get; set; }
    public string? Notes { get; set; }
    public short CurrentStatus { get; set; } = 1;
}
```

### Step 2: Location DTO
```csharp
public class UpsertReferralLocationRequestDto
{
    public Guid LocationId { get; set; }
    public Guid ReferralInfoId { get; set; } // 🔑 Foreign Key
    
    [Required]
    public string LocationName { get; set; }
    
    [Required]
    public string? Address { get; set; }
    
    [Required]
    public string? City { get; set; }
    
    [Required]
    public string? StateCode { get; set; }
    
    [Required]
    public string? ZipCode { get; set; }
    
    [Required]
    public string? CountryCode { get; set; }
    
    [Required]
    public string? Phone { get; set; }
    
    public string? PhoneCountryCode { get; set; }
    
    [Required, EmailAddress]
    public string? Email { get; set; }
}
```

### Step 3: Contact DTO
```csharp
public class UpsertReferralContactRequestDto
{
    public Guid ContactId { get; set; }
    public Guid ReferralInfoId { get; set; } // 🔑 Foreign Key
    
    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }
    
    [Required]
    public string? Title { get; set; }
    
    [Required]
    public string? Phone { get; set; }
    
    public string? PhoneCountryCode { get; set; }
    
    [Required, EmailAddress]
    public string? Email { get; set; }
    
    [Required]
    public string? PreferredContact { get; set; }
    
    public short ContactType { get; set; } = 0;
}
```

---

## API Service Methods

### Service Interface
```csharp
public interface IMasterService
{
    Task<Result<Guid>> UpsertReferralInfoAsync(UpsertReferralInfoRequestDto requestDto);
    Task<Result<Guid>> UpsertReferralLocationAsync(UpsertReferralLocationRequestDto requestDto);
    Task<Result<Guid>> UpsertReferralContactAsync(UpsertReferralContactRequestDto requestDto);
    Task<Result<ReferralByIdDto>> GetReferralById(Guid referralId);
}
```

### Implementation Example
```csharp
public async Task<Result<Guid>> UpsertReferralInfoAsync(UpsertReferralInfoRequestDto requestDto)
{
    var httpClient = _httpClientFactory.CreateClient("NexSchedApi");
    var result = new Result<Guid>();

    using var response = await httpClient
        .PostAsJsonAsync("api/Master/upsert-referral-info", requestDto)
        .ConfigureAwait(false);

    var apiResult = await response.Content
        .ReadFromJsonAsync<Result<Guid>>();

    if (apiResult != null)
        return apiResult;

    return result.ErrorResponse(
        ErrorCode.Unknown,
        "Invalid or empty response received."
    );
}
```

---

## Edit Mode Support

### Loading Existing Data
```csharp
protected override async Task OnInitializedAsync()
{
    isLoding = true;
    
    // Initialize defaults
    countryCodes = CountryMaskHelper.GetCountryCodes();
    await GetBusinessTypesAsync();
    
    referralRequest.PhoneCountryCode = "US";
    locationRequest.PhoneCountryCode = "US";
    contactRequest.PhoneCountryCode = "US";
    
    await LoadCountriesAsync();
    
    // 🔑 Check if editing existing record
    if (ReferralId.HasValue && ReferralId.Value != Guid.Empty)
    {
        await LoadReferralForEdit(ReferralId.Value);
    }
    
    isLoding = false;
}
```

### Load Data for Edit
```csharp
private async Task LoadReferralForEdit(Guid referralId)
{
    var response = await masterService.GetReferralById(referralId);
    
    if (!response.Success)
    {
        ShowErrorModal(response.Errors);
        return;
    }
    
    var data = response.Data;
    
    // Populate Step 1 fields
    referralRequest = new UpsertReferralInfoRequestDto
    {
        ReferralInfoId = data.Organization_Id,
        OrganizationName = data.Organization_Name,
        BussinessType = data.Business_Type,
        FirstName = data.First_Name,
        LastName = data.Last_Name,
        Email = data.Email,
        Phone = data.Phone,
        PhoneCountryCode = data.Phone_Country_Code,
        EinTaxId = data.Ein_TaxId,
        Notes = data.Note_Text
    };
    
    // Populate Step 2 fields
    locationRequest = new UpsertReferralLocationRequestDto
    {
        ReferralInfoId = data.Organization_Id,
        LocationId = data.Primary_Location_Id ?? Guid.Empty,
        LocationName = data.Primary_Location_Name,
        Address = data.Primary_Address,
        City = data.Primary_City,
        StateCode = data.Primary_State_Code,
        ZipCode = data.Primary_Zip_Code,
        CountryCode = data.Primary_Country_Code,
        Phone = data.Primary_Contact_Phone,
        PhoneCountryCode = data.Primary_Country_Code,
        Email = data.Primary_Location_Email
    };
    
    // Load states if country selected
    if (!string.IsNullOrWhiteSpace(locationRequest.CountryCode))
    {
        await LoadStatesAsync(locationRequest.CountryCode);
    }
    
    // Populate Step 3 fields
    contactRequest = new UpsertReferralContactRequestDto
    {
        ReferralInfoId = data.Organization_Id,
        ContactId = data.Primary_Contact_Id ?? Guid.Empty,
        FirstName = data.Primary_Contact_First_Name,
        LastName = data.Primary_Contact_Last_Name,
        Title = data.Primary_Contact_Title,
        Email = data.Primary_Contact_Email,
        Phone = data.Primary_Contact_Phone,
        PhoneCountryCode = data.Primary_Contact_Phone_Country_Code,
        PreferredContact = data.Primary_Contact_Preferred_Contact,
        ContactType = data.Primary_Contact_Type ?? 0
    };
    
    Step = 1; // Always start at first step
}
```

---

## Advanced Features

### 1. **Phone Number Masking by Country**

```csharp
private async Task OnChangeReferralPhoneCountryCode(string value)
{
    referralRequest.PhoneCountryCode = value;
    string maskPattern = CountryMaskHelper.GetPhoneMaskPattern(value);
    await JS.InvokeVoidAsync("applyMask", "referralPhoneInput", maskPattern);
}
```

### 2. **Country/State Cascade Selection**

```csharp
private async Task CountrySelected(string code)
{
    locationRequest.CountryCode = code;
    locationRequest.StateCode = string.Empty; // Reset state
    await LoadStatesAsync(code);
}

private async Task LoadStatesAsync(string countryCode)
{
    if (string.IsNullOrWhiteSpace(countryCode))
    {
        states = new();
        return;
    }
    
    var res = await masterService.GetStates(countryCode);
    
    if (!res.Success)
    {
        ShowErrorModal(res.Errors);
        return;
    }
    
    states = res.Data?.ToList() ?? new();
}
```

### 3. **Autocomplete Search**

```csharp
private Task<IEnumerable<string>> SearchCountries(string value, CancellationToken token)
{
    IEnumerable<string> result;
    
    if (string.IsNullOrWhiteSpace(value))
    {
        result = countries.Select(c => c.CountryCode);
    }
    else
    {
        result = countries
            .Where(c => c.CountryName.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Select(c => c.CountryCode);
    }
    
    return Task.FromResult(result);
}
```

---

## Navigation Helper

```csharp
private void GoToStep(int step)
{
    Step = step;
}

private void SwitchTab(int step)
{
    Step = step;
    
    // Reset country codes when switching
    if (Step == 1)
    {
        referralRequest.PhoneCountryCode = "US";
    }
    else if (Step == 2)
    {
        locationRequest.PhoneCountryCode = "US";
    }
    else if (Step == 3)
    {
        contactRequest.PhoneCountryCode = "US";
    }
}

private void NavigateBack()
{
    NavigationManager.NavigateTo("/referral");
}
```

---

## Key Benefits of This Pattern

### ✅ **Data Integrity**
- Each step's data is saved immediately
- No risk of losing data if user navigates away
- Database constraints are validated at each step

### ✅ **User Experience**
- Progressive completion feedback
- Can return to edit previous steps
- Clear visual indication of progress
- Success notifications at each step

### ✅ **Error Handling**
- Errors caught and displayed immediately
- User stays on the problematic step
- Can fix errors before proceeding

### ✅ **Backend Simplicity**
- Separate API endpoints for each entity
- No complex transaction handling needed
- Easier to test and maintain

### ✅ **Edit Mode Support**
- Can reload and edit existing data
- Same form works for create and update
- IDs are preserved for updates

---

## Best Practices

### 1. **Always Validate Before Saving**
```razor
<EditForm Model="@model" OnValidSubmit="SaveAndNext" novalidate>
    <DataAnnotationsValidator />
    <!-- Form fields -->
</EditForm>
```

### 2. **Capture IDs for Foreign Keys**
```csharp
referralId = referralResult.Data;
locationRequest.ReferralInfoId = referralId.Value;
contactRequest.ReferralInfoId = referralId.Value;
```

### 3. **Show Loading States**
```razor
<MudButton ButtonType="ButtonType.Submit" Disabled="@isSubmitting">
    @if (isSubmitting)
    {
        <MudProgressCircular Indeterminate="true" Size="Size.Small" Class="mr-2" />
        <span>Saving...</span>
    }
    else
    {
        <span>Next</span>
    }
</MudButton>
```

### 4. **Provide User Feedback**
```csharp
Snackbar.Add("Organization details saved successfully!", Severity.Success);
```

### 5. **Handle Errors Gracefully**
```csharp
if (!result.Success)
{
    isSubmitting = false;
    ShowErrorModal(result.Errors);
    return;
}
```

---

## Common Pitfalls to Avoid

❌ **Don't skip validation**
- Always use `DataAnnotationsValidator`
- Validate on submit, not just on field change

❌ **Don't forget to disable buttons during save**
- Set `Disabled="@isSubmitting"` on buttons
- Prevents duplicate submissions

❌ **Don't move to next step on error**
- Keep user on current step to fix errors
- Show clear error messages

❌ **Don't forget to pass foreign keys**
- Always set parent IDs before saving child records

❌ **Don't make the final "submit" save data**
- Final step should only redirect
- All data should already be saved

---

## Routing Setup

```csharp
@page "/referral-source/{ReferralId:guid?}"

[Parameter] 
public Guid? ReferralId { get; set; }
```

**Usage:**
- Create new: `/referral-source`
- Edit existing: `/referral-source/{guid}`

---

## Summary

This pattern provides a robust, user-friendly way to handle complex multi-step forms where:
- Each step represents a separate entity
- Data relationships exist between entities
- Users need immediate feedback
- Data must be persisted progressively

**Key Workflow:**
1. User fills Step 1 → Save → Get ID → Move to Step 2
2. User fills Step 2 → Save (with Step 1 ID) → Get ID → Move to Step 3
3. User fills Step 3 → Save (with Step 1 ID) → Get ID → Move to Step 4
4. User reviews → Confirm → Redirect (no save needed)

All data is safely stored in the database after each step, providing a seamless and secure user experience.

