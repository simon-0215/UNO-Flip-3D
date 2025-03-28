using UnityEngine;
using QFramework;
public class CardGameApp: Architecture<CardGameApp>
{
    protected override void Init()
    {
        this.RegisterModel(new CardGameModel());
    }
}

public struct DrawCardFromDeckEvent { }
public class DrawCardFromDeckCommand: AbstractCommand
{
    protected override void OnExecute()
    {
        this.SendEvent<DrawCardFromDeckEvent>();
    }
}

public struct UpdateMessageBoxEvent
{
    public string message;
}
public class UpdateMessageBoxCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        var model = this.GetModel<CardGameModel>();
        this.SendEvent(new UpdateMessageBoxEvent() { message = model.currentMessage });
    }
}

public struct AiSwitchPlayerEvent { }
public class AiSwitchPlayerCommand: AbstractCommand
{
    protected override void OnExecute()
    {
        this.SendEvent<AiSwitchPlayerEvent>();
    }
}

public struct UpdatePlayerHighlightsEvent{}
public class UpdatePlayerHighlightsCommand: AbstractCommand
{
    protected override void OnExecute()
    {
        this.SendEvent<UpdatePlayerHighlightsEvent>();
    }
}

public struct PlayCardEvent
{
    public CardDisplay cardDisplay;
    public Card card;
}
public class PlayCardCommand: AbstractCommand
{
    protected override void OnExecute()
    {
        var model = this.GetModel<CardGameModel>();
        this.SendEvent(new PlayCardEvent() { 
            cardDisplay = model.currentCardDisplay ,
            card = model.currentCard
        });
    }
}

public struct ChosenColourEvent
{
    public CardColour cardColour;
}
public class ChosenColourCommand: AbstractCommand
{
    protected override void OnExecute()
    {
        var model = this.GetModel<CardGameModel>();
        this.SendEvent(new ChosenColourEvent() { cardColour = model.currentCardColour });
    }
}