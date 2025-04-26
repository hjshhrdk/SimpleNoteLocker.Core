// JavaScript Custom Action (VERY LIMITED EXAMPLE - USE WITH CAUTION)

var session = installer.session; // Get the installer session object (if available)

try {
    var network = new ActiveXObject("WScript.Network"); // Try to create WScript.Network
    var userDomain = network.UserDomain; // Get the user's domain

    if (userDomain) {
        //  Basic string comparison (replace with your actual domain)
        var domainToCheck = "yourdomain.com";
        if (userDomain.toLowerCase() === domainToCheck.toLowerCase()) {
            session.Property("DOMAINCHECK") = "1"; // Set property for success
            session.LogMessage(1, "INFO: User is in the required domain: " + userDomain);
        } else {
            session.Property("DOMAINCHECK") = "0"; // Set property for failure
            session.LogMessage(1, "ERROR: User is not in the required domain.  Installation terminated.");
            session.Property("CustomActionReturnValue") = 1603; // MSI error code for fatal error
        }
    }
    else {
        session.Property("DOMAINCHECK") = "0"; // Set property for failure
        session.LogMessage(1, "ERROR: Could not determine user domain.  Installation terminated.");
        session.Property("CustomActionReturnValue") = 1603; // MSI error code
    }
}
catch (err) {
    // Handle errors (WScript.Network might not be available)
    session.Property("DOMAINCHECK") = "0"; // Set property for failure
    session.LogMessage(1, "ERROR: Error getting domain: " + err.message + ". Installation terminated.");
    session.Property("CustomActionReturnValue") = 1603; // MSI error code
}