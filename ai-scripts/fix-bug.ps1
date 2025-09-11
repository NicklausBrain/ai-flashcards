param(
    [Parameter(Mandatory = $true)]
    [string]$BugReportPath,

    [Parameter(Mandatory = $true)]
    [string]$BugId
)

# Load the bug report JSON
$BugReport = Get-Content $BugReportPath | ConvertFrom-Json

# Find the bug entry by bugId
$Bug = $BugReport | Where-Object { $_.bugId -eq $BugId }

if (-not $Bug) {
    Write-Error "Bug with ID $BugId not found in $BugReportPath"
    exit 1
}

# Construct the AI prompt for fixing the bug
# TypeScript
$Prompt = @"
# ROLE
You are an expert Senior Software Engineer with strong expertise in C#, Blazor, Git/Github, and automated bug fixing.

# CONTEXT
You will receive a bug report entry from a JSON file. The report includes the file path, line number, severity, bug type, description, and suggested fix.

Bug Report Entry:
$($Bug | ConvertTo-Json -Depth 10 -Compress)

# TASK
1. Open a new branch in the repository.
2. Apply a code fix to the file at `"filePath"` that resolves the bug described BUT DO NOT CHANGE FILE FORMATTING!.
3. Ensure that the fix follows secure coding best practices, maintains functionality, and includes tests if applicable.
4. Commit the change with a descriptive message.
5. Open a Pull Request with a clear explanation of the fix.
6. Update the `"prLink"` field of the bug entry in the JSON file (`$BugReportPath`) with the link to the created PR.

# OUTPUT FORMAT
Return only the updated JSON array (with the `"prLink"` field filled in for this bug). Do NOT output anything else.
"@

# Run gemini-cli with the constructed prompt
gemini $Prompt --yolo yolo