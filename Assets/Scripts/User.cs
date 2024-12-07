using UnityEngine;
public class User : MonoBehaviour
{
    public UserRole role; // roles = Student, Teacher, Parent
    public string username; // username for id

    public void AssignRole(UserRole assignedRole)
    {
        role = assignedRole;
        Debug.Log($"User assigned role: {role}");
    }

    public void SetUsername(string name)
    {
        username = name;
    }

    public string GetRoleDisplay()
    {
        return $"{username} - {role}";
    }
}


