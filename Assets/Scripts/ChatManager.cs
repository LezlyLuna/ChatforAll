using UnityEngine;
using Unity.Netcode;
using TMPro; 

public class ChatManager : NetworkBehaviour
{
    public TMP_InputField chatInputField; //field showing typing messages
    public TextMeshProUGUI chatDisplay;  //area that shows the messages
    public TextMeshProUGUI typingIndicator; // is typing... display!!

    private float typingResetDelay = 2f; // this should delay before resetting the typing status
    private Coroutine typingResetCoroutine;// this coroutine manages the tyoing status reset (hopefully)

    private void Start()
    {
        // assigning input field and making listeners for typing and message submission
        if (chatInputField != null)
            {
                Debug.Log("Chat Input Field assigned.");
                chatInputField.onSubmit.AddListener(SubmitChatMessage);// this is handling submitting the chat
                chatInputField.onValueChanged.AddListener(OnTyping); // the new typing indicator!!!
            }

        // from chatgpt to check if this is the server and the NetworkObject is not spawned, cuz i had errors
        if (IsServer && !NetworkObject.IsSpawned)
            {
                NetworkObject.Spawn(); // Spawn the object on the server
                Debug.Log("ChatManager spawned on the server.");
            }
    }

    public override void OnDestroy()
    {       // this helps with cleanup to remove any listners to avoid 'memory leaks'
        if (chatInputField != null)
        {
            chatInputField.onSubmit.RemoveListener(SubmitChatMessage);
            chatInputField.onValueChanged.RemoveListener(OnTyping);
        }

        base.OnDestroy(); //more cleanup
    }


    //time to handle some incomming messages
    [ServerRpc(RequireOwnership = false)]
    private void SubmitMessageServerRpc(string message, ulong senderId)
    {
        //here i format the message with the ID (which should be teacher and student but oh well
        string formattedMessage = $"User {senderId}: {message}";
        //sending all the formatted messages to everyone in the chat
        DisplayMessageClientRpc(formattedMessage);
        //updating the typing status to false once the message is sent
        UpdateTypingStatusClientRpc(senderId, false); // new here too Stop typing indication when a message is sent

    }


    //displaying all messages to all clients
    [ClientRpc]
    private void DisplayMessageClientRpc(string message)
    {
        //append the new messages to the chat display
        chatDisplay.text += $"\n{message}";
    }

    //update typing status for typing
    [ServerRpc(RequireOwnership = false)]
    private void UpdateTypingStatusServerRpc(ulong senderId, bool isTyping)
    {
        //show typing status to all users
        UpdateTypingStatusClientRpc(senderId, isTyping);
    }

    //show and hid typing message
    [ClientRpc]
    private void UpdateTypingStatusClientRpc(ulong senderId, bool isTyping)
    {
        if (typingIndicator == null) return;// doing nothing if there is no typing indicator

        //if they are typing then show it
        if (isTyping)
        {
            typingIndicator.text = $"User {senderId} is typing...";
        }
        else
        {
            typingIndicator.text = string.Empty;//clear the typing message
        }
    }

    //user typing in input field
    private void OnTyping(string input)
    {
        //input field is empty, stop the typing message
        if (string.IsNullOrEmpty(input))
        {
            UpdateTypingStatusServerRpc(NetworkManager.Singleton.LocalClientId, false);
            return;
        }

        // Notify others that this user is typing
        UpdateTypingStatusServerRpc(NetworkManager.Singleton.LocalClientId, true);

        // Reset typing status after a delay
        if (typingResetCoroutine != null) StopCoroutine(typingResetCoroutine);
        typingResetCoroutine = StartCoroutine(ResetTypingStatus());
    }

    //reset typing status after the delay
    private System.Collections.IEnumerator ResetTypingStatus()
    {
        yield return new WaitForSeconds(typingResetDelay);
        UpdateTypingStatusServerRpc(NetworkManager.Singleton.LocalClientId, false);//telling the server to stop typing
    }

//sending the message
    public void SubmitChatMessage(string message)
    {
        Debug.Log($"Message submitted: {message}");
        //doing nothing if the message is not there
        if (string.IsNullOrWhiteSpace(message)) return;
        //sending message to the server
        SubmitMessageServerRpc(message, NetworkManager.Singleton.LocalClientId);
        chatInputField.text = string.Empty; //clearing input field after sending the message
    }
}
