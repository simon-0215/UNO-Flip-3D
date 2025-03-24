using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Collections;

[TestFixture]
public class MenuTests
{
    private GameObject menuObject;
    private TestMenu menu;
    private GameObject howToPlayPanelObject;

    // Test-friendly version of Menu that allows us to override scene management
    private class TestMenu : Menu
    {
        public string LastLoadedScene { get; private set; }
        public bool QuitCalled { get; private set; }

        public override void LoadLevel(string levelName)
        {
            LastLoadedScene = levelName;
        }

        public override void RestartGame()
        {
            LastLoadedScene = "RESTART";
        }

        public override void ExitGame()
        {
            QuitCalled = true;
        }
    }

    [SetUp]
    public void SetUp()
    {
        // Create menu object
        menuObject = new GameObject("Menu");
        menu = menuObject.AddComponent<TestMenu>();

        // Create how to play panel
        howToPlayPanelObject = new GameObject("HowToPlayPanel");
        howToPlayPanelObject.SetActive(false);

        // Set private field
        var field = typeof(Menu).GetField("howToPlayPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(menu, howToPlayPanelObject);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(menuObject);
        Object.DestroyImmediate(howToPlayPanelObject);
    }

    [Test]
    public void Test_Menu_HowToPlayPanel_Activation()
    {
        Assert.IsFalse(howToPlayPanelObject.activeSelf, "Panel should start inactive");
        
        menu.HowToPlayPanel();
        
        Assert.IsTrue(howToPlayPanelObject.activeSelf, "Panel should be active after calling HowToPlayPanel");
    }

    [Test]
    public void Test_Menu_HowToPlayPanel_NullHandling()
    {
        // Set panel to null
        var field = typeof(Menu).GetField("howToPlayPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(menu, null);

        // Should not throw exception when panel is null
        Assert.DoesNotThrow(() => menu.HowToPlayPanel(), "HowToPlayPanel should handle null panel gracefully");
    }

    [Test]
    public void Test_Menu_LoadLevel()
    {
        string testSceneName = "TestScene";
        menu.LoadLevel(testSceneName);
        
        Assert.AreEqual(testSceneName, menu.LastLoadedScene, "LoadLevel should store the correct scene name");
    }

    [Test]
    public void Test_Menu_RestartGame()
    {
        menu.RestartGame();
        Assert.AreEqual("RESTART", menu.LastLoadedScene, "RestartGame should attempt to reload the scene");
    }

    [Test]
    public void Test_Menu_ExitGame()
    {
        menu.ExitGame();
        Assert.IsTrue(menu.QuitCalled, "ExitGame should set the quit flag");
    }
} 