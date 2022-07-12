The goal of this project is to compare different ways of reading the [rating](https://docs.microsoft.com/en-us/windows/win32/properties/props-system-rating) (or any other extended file attribute) of a file in order to find the most efficient one in terms of performance.

Each file contains a different "method" of completing the same action, which is to:
1. Read the rating of a specific file 5000 times
2. If the file is unrated (rating is equals to 0 or null), put the name of the file in a list
3. Output the time in milliseconds that was needed to execute the benchmark, in a messagebox.

**Important notes:**

* The `Microsoft.WindowsAPICodePack-Core` and `Microsoft-WindowsAPICodePack-Shell` NuGet packages are required to run Method #1, but may also be required for other methods. Follow [this guide](https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio) to add NuGet packages to your solution in Visual Studio.
* Method #2, #3 and #4 require the use of the reference `Microsoft Shell Controls and Automation`. You can find more information on how to add references to your solution in Visual studio with [this guide](https://docs.microsoft.com/en-us/visualstudio/ide/how-to-add-or-remove-references-by-using-the-reference-manager?view=vs-2022) or [this answer on StackOverflow](https://stackoverflow.com/a/18894720).
* Method #5 requires the use of the the references `PresentationCore`, `System.Xaml`and `WindowsBase`.
* I am not able to get Method #5 to compile at the moment, probably because of my incorrect implementation I get multiple definition errors.
* I did not come up with most of the code to read the rating of a file, apart from the one in Method #1. Check the [StackOverflow post I made](https://stackoverflow.com/questions/72925511/how-to-read-the-rating-of-a-file-efficiently-in-a-c-sharp-winforms-app-or-other) to see sources to where I've taken the code from from different posts that have been made on similar subjects in the past.
```
