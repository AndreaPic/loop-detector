# Distributed Loop

What distributed loop is?

It's like a infinite loop such

```C#
while(true)
{

} 
```

and has some aspects like infinite recursion

```C#
void foo()
{
    foo();
}
```

Infinite recursion stops with stack overflow error but distributed loop don't :(

To imagine what distributed is you have to think in multidimensional way.

In this issue there are at least 2 process or services that conmunicate with each other.
They can communicate in many way such socket, http, web api etc...