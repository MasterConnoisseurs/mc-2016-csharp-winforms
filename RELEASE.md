# 🧺 MC Laundry Shop Application 🧼

This guide will help you install and run the application on your Windows machine.

## Download Links

* **Database Script:** [Database Script](https://drive.google.com/file/d/1XloNdcnoJ3eb2VBiZbIQIv139yICfCOq/view?usp=sharing) 📄
* **Application RAR File:** [Application RAR File](https://drive.google.com/file/d/1moU0u8ljFmX52RAH-apJhv2MktUyE8mT/view?usp=sharing) 📦

## Installation Summary

Here's a quick overview of the steps you'll follow. Click on any step to jump to its detailed instructions:

* **[Step 1: Install .NET Framework 4.6.2 or Higher](#step-1-install-net-framework-462-or-higher)**
* **[Step 2: Install MySQL Server 8.0.36 or Higher](#step-2-install-mysql-server-8036-or-higher)**
* **[Step 3: Run the Database Script in MySQL](#step-3-run-the-database-script-in-mysql)**
* **[Step 4: Extract mc-2016-csharp-winforms.rar](#step-4-extract-mc-2016-csharp-winformsrar)**
* **[Step 5: Edit the Connection String](#step-5-edit-the-connection-string)**
* **[Step 6: Run the Application Installer](#step-6-run-the-application-installer)**

---

## 🛠️ Prerequisites

Before proceeding with the installation, ensure your system meets the following requirements:

* **Operating System:** Windows 7 or higher (Windows 10/11 recommended) 💻
* **Internet Connection:** Required for downloading prerequisites and application files. 🌐

---

## 🚀 Installation Guide

Please follow these steps in the specified order to successfully set up and run the MC Laundry Shop application:

---

### Step 1: Install .NET Framework 4.6.2 or Higher

The MC Laundry Shop application is built using the .NET Framework and requires it to be installed on your system.

1.  **Check if installed:**
    * Go to `Control Panel` > `Programs` > `Programs and Features`.
    * Look for "Microsoft .NET Framework 4.6.2" or any higher 4.x version (e.g., 4.7.2, 4.8). ✅
2.  **If not installed:**
    * Download the official **Microsoft .NET Framework 4.8 Runtime** (recommended, as it's backward compatible with 4.6.2) from the Microsoft website. Search for "Download .NET Framework 4.8 Runtime". ⬇️
    * Run the downloaded installer and follow the on-screen instructions. A computer restart may be required. 🔄

---

### Step 2: Install MySQL Server 8.0.36 or Higher

The application relies on a MySQL database server to store and manage its data. 💾

1.  **If not installed:**
    * Download the **MySQL Community Server 8.0.36** (or a higher version) from the official MySQL website: [https://dev.mysql.com/downloads/mysql/](https://dev.mysql.com/downloads/mysql/) 🌐
    * Choose the "MySQL Installer for Windows" for a guided setup process.
    * Run the installer and follow the instructions to set up the MySQL server.
    * **Important:** During the MySQL setup, you will be prompted to set a password for the `root` user. **Remember this password**, as it will be used in a later step to configure the application's connection. 🔑

---

### Step 3: Run the Database Script in MySQL

After MySQL Server is installed and running, you need to set up the specific database and tables required by the MC Laundry Shop application. 📋

1.  **Locate the Database Script:**
    * Download the database script from the link provided above: [Database Script](https://drive.google.com/file/d/1XloNdcnoJ3eb2VBiZbIQIv139yICfCOq/view?usp=sharing) 📄
2.  **Execute the Script:**
    * Open a MySQL client tool (e.g., MySQL Workbench, or the MySQL Command Line Client, which comes with MySQL Server installation).
    * Log in to your MySQL server using the `root` user and the password you set in **Step 2**. 🔒
    * Execute the downloaded database script. This will create the necessary database named `mc_laundryclean` and its tables, as expected by the application. ✨

---

### Step 4: Extract `mc-2016-csharp-winforms.rar`

This RAR file contains the application's installer and core files. 📦

1.  **Locate the RAR file:** Download the application RAR file from the link provided above: [Application RAR File](https://drive.google.com/file/d/1moU0u8ljFmX52RAH-apJhv2MktUyE8mT/view?usp=sharing) 📁
2.  **Extract the contents:**
    * Right-click on the `.rar` file.
    * Select "Extract Here" or "Extract to `mc-2016-csharp-winforms\`" using an archiving tool like WinRAR or 7-Zip. 📂
    * This will create a new folder (e.g., `mc-2016-csharp-winforms`) containing `setup.exe` and an `Application Files` subfolder.

---

### Step 5: Edit the Connection String

The application needs to be configured with the correct details to connect to your specific MySQL database instance. ⚙️

1.  **Navigate to the Application Files:**
    * Go to the extracted folder: `[Extracted Folder]\Application Files\MC.LaundryShop.App_1_0_0_0\`
        * (Replace `[Extracted Folder]` with the actual name of the folder created in Step 4, such as `mc-2016-csharp-winforms`).
2.  **Rename the configuration file:**
    * Find the file named `MC.LaundryShop.App.exe.config.deploy`.
    * **Rename this file** to `MC.LaundryShop.App.exe.config` (remove the `.deploy` extension). 📝
3.  **Open and Edit:**
    * Open `MC.LaundryShop.App.exe.config` using Notepad or any text editor.
    * Locate the `<connectionStrings>` section. It will look exactly like this:

        ```xml
        <connectionStrings>
            <add name="MySQLConnection" connectionString="Server=localhost;Port=3306;Database=mc_laundryclean;Uid=root;Pwd=root_password;" providerName="MySql.Data.MySqlClient" />
        </connectionStrings>
        ```
4.  **Modify the `connectionString` based on your MySQL `root` user's password:**
    * Change `root_password` to the **actual password** you set for the MySQL `root` user during installation (in **Step 2**). ✏️
    * **If your MySQL Server is on a different computer** on your network (not the same machine where you're installing the app), change `Server=localhost` to its IP address (e.g., `Server=192.168.1.100`) or hostname. Keep `Port=3306` unless you've changed the default MySQL port. 📍
5.  **Save** the `MC.LaundryShop.App.exe.config` file. ✅

---

### Step 6: Run the Application Installer

Finally, run the ClickOnce installer to deploy the application to your system. ▶️

1.  **Go back to the main extracted folder** (e.g., `mc-2016-csharp-winforms`).
2.  **Double-click on `setup.exe`**. 🏃‍♂️
3.  Follow the on-screen prompts to install the MC Laundry Shop application. 📩

---

## ✨ After Installation

Once installed, you should find a shortcut to "MC Laundry Shop App" in your Start Menu or on your Desktop. Double-click the shortcut to launch the application.

If all steps were followed correctly, the application should now connect to your `mc_laundryclean` database using the configured connection and be ready for use! 🎉
