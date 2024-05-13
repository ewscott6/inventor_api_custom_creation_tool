# Custom Flanged Devices Builder Widget Backend - README

## Overview

This project is a C# backend server application designed to support the development of a custom flanged devices builder widget for a website. The backend interacts with Autodesk Inventor to generate and manage 3D models of flanged components and assemblies. It includes functionalities to select, configure, and constrain flanged components using a graphical user interface.

## Project Structure

The project consists of the following main components:

1. **Program1.cs**: The main program file that initializes the Inventor application, sets up the assembly environment, and manages the user interface for flange selection and configuration.
2. **SQLite.cs**: Contains classes and methods for interacting with an SQLite database that stores information about flanged components.
3. **FilePaths.cs**: Defines static file paths used throughout the project to locate parts, assemblies, and other resources.

## File Descriptions

### Program1.cs

- Initializes the Inventor application and sets up references to the active assembly document.
- Provides a user interface for selecting and configuring flanges, including selecting flange types, materials, and tube outer diameters (OD).
- Manages the addition of flanged components to an assembly and applies constraints to position and orient these components correctly.
- Includes functions for checking and resolving interferences between components, updating part parameters, and restoring assembly defaults.

### SQLite.cs

- Defines `FlangeIdentifier` and `FlangeDatabase` classes for managing flanged component data.
- Provides methods to retrieve and filter flange data from the SQLite database based on user selections.
- Implements functionalities to get unique values for flange attributes (type, material, tube OD) and to update options dynamically based on current selections.

### FilePaths.cs

- Contains static file path definitions used to locate various resources required by the application, such as part files, face area data, and standard pipe size tables.

## Getting Started

### Prerequisites

- Autodesk Inventor installed and licensed.
- .NET Framework installed.
- SQLite database containing flange data.

### Setup

1. **Clone the repository**:
    ```bash
    git clone <repository-url>
    ```

2. **Open the project in Visual Studio**:
    - Open Visual Studio.
    - Select "Open a project or solution" and navigate to the cloned repository.

3. **Configure file paths**:
    - Ensure that the file paths defined in `FilePaths.cs` are correct and point to the appropriate directories on your system.

4. **Run the application**:
    - Set `Program1.cs` as the startup file.
    - Build and run the project in Visual Studio.

### Usage

1. **Main Menu**:
    - The application starts with a main menu displaying options to select flanges for different positions on a tubing cross.
    - Use the menu to select and configure flanges.

2. **Flange Selection**:
    - Select flange types, materials, and tube OD from the available options.
    - The application filters options dynamically based on current selections.

3. **Assembly and Constraints**:
    - Selected flanges are added to the assembly with appropriate constraints.
    - The application checks for interferences and updates part parameters as needed.

4. **Restore Defaults**:
    - Option to restore the assembly and part parameters to their default values.

### Database Interaction

- The application interacts with an SQLite database to retrieve and store flange data.
- Ensure that the database file is located at the path specified in `FilePaths.cs` and contains the required tables and data.

## Additional Information

- **Logging**: The application logs messages to the console for various operations, such as adding constraints, checking for interferences, and updating parameters.
- **Error Handling**: Basic error handling is implemented to manage exceptions during file operations and database interactions.

## Future Enhancements

- Implement a graphical user interface (GUI) for easier interaction.
- Add support for additional flange types and configurations.
- Improve error handling and logging mechanisms.
- Integrate with a web-based frontend for real-time user interactions.

## License

This project is licensed under the MIT License. See the LICENSE file for details.

## Acknowledgements

- Figgle for ASCII art generation.
- SQLite for database management.
- Autodesk Inventor for CAD and assembly management.

---

For any issues or contributions, please open an issue or submit a pull request on the project's GitHub repository.