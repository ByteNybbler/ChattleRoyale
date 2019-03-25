// Author(s): Paul Calande
// Unit test script for PAUtiLib.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAUtiLibUnitTest : MonoBehaviour
{
    // The number of the current test in the current suite.
    int currentTestNumber = 0;
    // The name of the current test suite.
    string currentSuiteName = "UNNAMED";

    // Test an assertion.
    void Test(bool assertion,
        string extraInfo = "Assertion failed.")
    {
        ++currentTestNumber;
        if (!assertion)
        {
            Debug.LogError("PAUtiLib Unit Test: "
                + currentSuiteName
                + currentTestNumber + " failed to pass.\n"
                + extraInfo);
        }
    }

    void TestEqual<T>(T lhs, T rhs)
    {
        Test(UtilGeneric.IsEqualTo(lhs, rhs), "LHS = " + lhs + "; RHS = " + rhs);
    }

    // Begin a new test suite.
    void EnterNewSuite(string newSuiteName)
    {
        currentSuiteName = newSuiteName;
        currentTestNumber = 0;
    }

    void Start()
    {
        EnterNewSuite("Angles");
        Angle a = Angle.FromDegrees(20.0f);
        Angle b = Angle.FromDegrees(310.0f);
        TestEqual(Angle.GetLargerDistance(a, b).GetDegreesUnsigned(), 290.0f);
        TestEqual(Angle.GetSmallerDistance(b, a).GetDegreesUnsigned(), 70.0f);
        TestEqual(Angle.FromHeadingVector(1.0f, 0.0f).GetDegreesUnsigned(), 0.0f);
        TestEqual(Angle.FromHeadingVector(0.0f, 1.0f).GetDegreesUnsigned(), 90.0f);
        TestEqual(Angle.FromHeadingVector(-1.0f, 0.0f).GetDegreesUnsigned(), 180.0f);
        TestEqual(Angle.FromHeadingVector(0.0f, -1.0f).GetDegreesUnsigned(), 270.0f);
        Test(Angle.FromDegrees(30.0f).IsCoterminal(Angle.FromDegrees(-330.0f)));
        Test(Angle.FromDegrees(30.0f).IsCoterminal(Angle.FromDegrees(390.0f)));

        EnterNewSuite("Generics");
        TestEqual(UtilGeneric.Add<int, int, int>(5, 7), 12);
        Vector3 start = new Vector3(1, 0, 0);
        Vector3 end = new Vector3(16, 0, 0);
        Vector3 velocity = new Vector3(5, 0, 0);
        TestEqual(UtilVelocity.GetVelocity(start, end, 3.0f), velocity);
        TestEqual(UtilVelocity.GetFuturePosition(2.0f, 3.0f, 6.0f), 20.0f);
    }
}