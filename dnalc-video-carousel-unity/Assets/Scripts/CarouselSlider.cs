using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CarouselSlider : MonoBehaviour
{
    [Header("Content Viewport")]
    public RawImage contentDisplay;
    public List<GameObject> contentPanels;

    [Header("Pagination Buttons")]
    public Button nextButton;
    public Button prevButton;

    [Header("Page Settings")]
    public bool useTimer = false;
    public bool isLimitedSwipe = false;
    public float autoMoveTime = 5f;
    private float timer;
    public int currentIndex = 0;
    public float swipeThreshold = 50f;
    private Vector2 touchStartPos;

    public Animator rightArrow;
    public Animator leftArrow;

    private int lastIndex = 0;

    // Reference to the RectTransform of the content area
    public RectTransform contentArea;

    void Start()
    {
        nextButton.onClick.AddListener(NextContent);
        prevButton.onClick.AddListener(PreviousContent);

        // Display initial content
        ShowContent();

        // Start auto-move timer if enabled
        if (useTimer)
        {
            timer = autoMoveTime;
            InvokeRepeating("AutoMoveContent", 1f, 1f); // Invoke every second to update the timer
        }
    }

    IEnumerator SmoothFill(RawImage image, float targetFillAmount, float duration)
    {
        float startFillAmount = image.uvRect.width;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float newFill = Mathf.Lerp(startFillAmount, targetFillAmount, elapsedTime / duration);
            image.uvRect = new Rect(0, 0, newFill, 1);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        image.uvRect = new Rect(0, 0, targetFillAmount, 1);
    }

    void Update()
    {
        // Detect swipe input only within the content area
        DetectSwipe();
    }

    void DetectSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
            HideArrows(); // Hide arrows immediately when the user starts dragging
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 touchEndPos = Input.mousePosition;
            float swipeDistance = touchEndPos.x - touchStartPos.x;

            // Check if the swipe is within the content area bounds
            if (Mathf.Abs(swipeDistance) > swipeThreshold && IsTouchInContentArea(touchStartPos))
            {
                if (isLimitedSwipe && ((currentIndex == 0 && swipeDistance > 0) || (currentIndex == contentPanels.Count - 1 && swipeDistance < 0)))
                {
                    // Limited swipe is enabled, and at the edge of content
                    return;
                }

                if (swipeDistance > 0)
                {
                    PreviousContent();
                }
                else
                {
                    NextContent();
                }
            }
        }
    }

    // Check if the touch position is within the content area bounds
    bool IsTouchInContentArea(Vector2 touchPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(contentArea, touchPosition);
    }

    void AutoMoveContent()
    {
        timer -= 1f; // Decrease timer every second

        if (timer <= 0)
        {
            timer = autoMoveTime;
            NextContent();
        }
    }

    void NextContent()
    {
        currentIndex = (currentIndex + 1) % contentPanels.Count;
        ShowContent();
    }

    void PreviousContent()
    {
        currentIndex = (currentIndex - 1 + contentPanels.Count) % contentPanels.Count;
        ShowContent();
    }

    void ShowContent()
    {
        // Ensure arrows are visible and active before playing their animations
        if (rightArrow != null && !rightArrow.gameObject.activeSelf)
        {
            StartCoroutine(ShowArrowsAfterDelay(3f));
        }

        if (leftArrow != null && !leftArrow.gameObject.activeSelf)
        {
            StartCoroutine(ShowArrowsAfterDelay(3f));
        }

        // Play arrow animation based on direction
        if (currentIndex > lastIndex && rightArrow != null)
        {
            rightArrow.Play("ArrowRight", 0, 0f); // "ArrowRight" = your actual animation state name
        }
        else if (currentIndex < lastIndex && leftArrow != null)
        {
            leftArrow.Play("ArrowRight", 0, 0f); // Ensure this is the correct animation for left arrow
        }

        lastIndex = currentIndex;
        

        for (int i = 0; i < contentPanels.Count; i++)
        {
            bool isActive = i == currentIndex;
            contentPanels[i].SetActive(isActive);

            if (isActive)
            {
                timer = autoMoveTime;
            }
        }
    }

    void HideArrows()
    {
        // Hide arrows immediately when the user drags
        if (rightArrow != null)
        {
            rightArrow.gameObject.SetActive(false);
        }

        if (leftArrow != null)
        {
            leftArrow.gameObject.SetActive(false);
        }
    }

    // Coroutine to show the arrows after a specified delay
    IEnumerator ShowArrowsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (rightArrow != null && !rightArrow.gameObject.activeSelf)
        {
            rightArrow.gameObject.SetActive(true); // Reactivate right arrow
            rightArrow.Play("ArrowRight", 0, 0f); // Play right arrow animation
        }

        if (leftArrow != null && !leftArrow.gameObject.activeSelf)
        {
            leftArrow.gameObject.SetActive(true); // Reactivate left arrow
            leftArrow.Play("ArrowRight", 0, 0f); // Play left arrow animation
        }
    }

    public void SetCurrentIndex(int newIndex)
    {
        if (newIndex >= 0 && newIndex < contentPanels.Count)
        {
            currentIndex = newIndex;
            ShowContent();
        }
    }
}
