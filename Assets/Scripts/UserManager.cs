using UnityEngine;

public class UserManager : MonoBehaviour
{
    private int userCount = 0; // Tracks the number of connected users

    public void OnUserJoin(GameObject userObject)
    {
        User user = userObject.GetComponent<User>();

        // Assign role based on user count
        UserRole assignedRole;
        if (userCount == 0)
        {
            assignedRole = UserRole.Teacher; // first user is the teacher
        }
        else
        {
            assignedRole = UserRole.Parent; // other users are parents
        }

        // Assign the role and increment the user count
        user.AssignRole(assignedRole);
        userCount++;

        // Assign a simple username
        user.username = assignedRole == UserRole.Teacher
            ? "Teacher"
            : $"Parent_{userCount}";

        Debug.Log($"User {user.username} joined with role: {assignedRole}");
    }
}

