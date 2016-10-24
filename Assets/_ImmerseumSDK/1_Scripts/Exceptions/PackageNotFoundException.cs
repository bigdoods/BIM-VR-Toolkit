using UnityEngine;
using System.Collections;

public class PackageNotFoundException : System.Exception {
    public PackageNotFoundException() {
    }

    public PackageNotFoundException(string message) {
    }

    public PackageNotFoundException(string message, System.Exception inner) : base(message, inner) {
    }
}
