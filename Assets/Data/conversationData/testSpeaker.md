[cStart] 
    {[3][name|Check In]}
    dude: 
    [r] : Hi
    [r] : Hey
    [r] : yo!
    how's it going?
    player: p good tbh
    dude: nice

[cStart]
    [hadDrink] dude: You already had a drink, finish that one first. [cEnd]

    dude: yoo what do u want 2 drink.
    player:ummmm...
    [c] : red bull [goto|2]
    [c] : coke [goto|1]
    [c] : Nothing for me thanks [goto|3]

    [label|1] 
    Coke, for sure [set|hadDrink]
    dude: radical. fizzy yep yep. [cEnd]

    [label|2]
    I'll have a red bull please[set|hadDrink]
    dude: caffeine will kill u u know [cEnd]

    [label|3] 
    I'm alright, maybe later
    dude: sure man, whatever float your boat

[cStart]
    { [1] [!died] }
    test: wow, you didn't die!
    player: I did now. [set|died] 


[cStart]
    [died] test: Oh, I'm sorry you died.
    [died] player: me to for real for real.
    test: what brings you here today?. 
    player: not much tbh


[cStart]
    {[died]}
    test: if we're talking, u must have died...