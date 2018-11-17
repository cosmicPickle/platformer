using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConversationUI : MonoBehaviour {

    #region Singleton
    public static ConversationUI instance;

    void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
                Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    public delegate void OnAnswer(DialogueDataNode answer = null);
    public GameObject conversationPanel;

    protected TextMeshProUGUI content;
    protected Button next;
    protected Transform answersPanel;
    protected Button[] answerButtons;

    private void Start()
    {
        content = conversationPanel.GetComponentInChildren<TextMeshProUGUI>();
        next = conversationPanel.GetComponentInChildren<Button>();
        answersPanel = conversationPanel.transform.Find("Answers");
        answerButtons = answersPanel.GetComponentsInChildren<Button>();
    }

    public void Show()
    {
        conversationPanel.SetActive(true);
    }

    public void Hide()
    {
        conversationPanel.SetActive(false);
    }

    public void ShowTextNode(DialogueDataNode node, UnityAction onNext)
    {
        content.text = node.text;

        next.gameObject.SetActive(true);
        answersPanel.gameObject.SetActive(false);

        next.onClick.RemoveAllListeners();
        next.onClick.AddListener(onNext);
    }

    public void ShowQuestionNode(DialogueDataNode node, List<DataGraphNode> answers, OnAnswer onAnswer)
    {
        content.text = node.text;

        next.gameObject.SetActive(false);
        answersPanel.gameObject.SetActive(true);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i >= answers.Count)
            {
                answerButtons[i].gameObject.SetActive(false);
                continue;
            }

    
            answerButtons[i].GetComponentInChildren<Text>().text = ((DialogueDataNode)answers[i]).text;
            answerButtons[i].gameObject.SetActive(true);

            answerButtons[i].onClick.RemoveAllListeners();

            DialogueDataNode answer = (DialogueDataNode)answers[i];
            answerButtons[i].onClick.AddListener(() =>
            {
                onAnswer(answer);
            });
        }
    }
}
