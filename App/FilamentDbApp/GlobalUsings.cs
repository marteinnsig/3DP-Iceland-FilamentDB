// System.IO.Path conflicts with System.Windows.Shapes.Path in WPF source files.
// Keep these aliases project-wide so new partial classes and services use one
// unambiguous convention without depending on file-local using directives.
global using IOPath = System.IO.Path;
global using IOFile = System.IO.File;
global using IODirectory = System.IO.Directory;
