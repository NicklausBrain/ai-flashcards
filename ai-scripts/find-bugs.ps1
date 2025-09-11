param(
    [Parameter(Mandatory = $true)]
    [string]$Path
)

# Prepare the audit prompt with the provided path
# TypeScript, Node.js
$Prompt = @"
# ROLE
You are an expert Senior Software Engineer and code auditor with deep expertise in C#, Blazor, and backend security best practices.

# CONTEXT
You will be analyzing the following C# file(s) from our backend codebase.
Path: `<$Path>`

# TASK
Your task is to perform a thorough code audit of the provided file(s). Identify potential bugs, logic errors, security vulnerabilities (like improper input validation), performance bottlenecks, and deviations from best practices (e.g., improper error handling, race conditions, potential null pointer exceptions).

# OUTPUT FORMAT
For each issue you find, create a JSON object. Combine all objects into a single JSON array. Do NOT add any explanations or text outside of the final JSON array.

The JSON schema for each object must be as follows:
{
  "bugId": "A unique UUID for this bug",
  "filePath": "The full path to the file where the bug was found",
  "lineNumber": "The specific line number where the issue begins",
  "severity": "A string: 'Critical', 'High', 'Medium', or 'Low'",
  "bugType": "A short category, e.g., 'Unhandled Exception', 'Security Vulnerability', 'Logic Error', 'Performance Issue'",
  "description": "A clear, concise explanation of the bug and why it's a problem.",
  "suggestedFix": "A detailed, actionable suggestion on how to fix the code.",
  "prLink": null
}

# ACTION
Begin the analysis now and generate the JSON output in the file named by a template: ai-bug-report-dd-mm-yyyy.json
"@

# Run gemini-cli with the constructed prompt
# --prompt 
gemini $Prompt --yolo yolo