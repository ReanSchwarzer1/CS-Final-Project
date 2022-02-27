using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

// Reference: Sebastian Schusman
public class Jumper : Agent
{
    [SerializeField] private float jumpForce;
    [SerializeField] private KeyCode jumpKey;
    
    private bool jumpIsReady = true;
    private Rigidbody rBody;
    private Vector3 startingPosition;
    private int score = 0;
    
    public event Action OnReset;


    private void OnCollisionEnter(Collision collidedObj)
    {
        if (collidedObj.gameObject.CompareTag("Street"))
            jumpIsReady = true;

        else if (collidedObj.gameObject.CompareTag("Mover") || collidedObj.gameObject.CompareTag("DoubleMover"))
        {
            AddReward(-1.0f);
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider collidedObj)
    {
        if (collidedObj.gameObject.CompareTag("score"))
        {
            AddReward(0.1f);
            score++;
            ScoreCollector.Instance.AddScore(score);
        }
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        if (Mathf.FloorToInt(vectorAction[0]) == 1)
            Jump();
    }

    public override void OnEpisodeBegin()
    {
        Reset();
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = 0;
        
        if (Input.GetKey(jumpKey))
            actionsOut[0] = 1;
    }

    private void Jump()
    {
        if (jumpIsReady)
        {
            rBody.AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
            jumpIsReady = false;
        }
    }

    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
        startingPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (jumpIsReady)
            RequestDecision();
    }

    private void Reset()
    {
        score = 0;
        jumpIsReady = true;
        
        //Reset Movement and Position
        transform.position = startingPosition;
        rBody.velocity = Vector3.zero;
        
        OnReset?.Invoke();
    }

 
}


/*

local os = os
local math = math
local io = io

math.randomseed(os.time())

local charlist = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890_"
local charlist_length = string.len(charlist)

local header_charlist = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_"
local header_charlist_length = string.len(header_charlist)

local function getChar(chars, charscount, i)
    local idx = i % charscount + 1
    return string.sub(chars, idx, idx);
end

local function getRandomChar()
    return getChar(charlist, charlist_length, math.random(1, charlist_length))
end

local function getRandomHChar()
    return getChar(header_charlist, header_charlist_length, math.random(1, charlist_length))
end

local function getRandomName(l)
    local r = getRandomHChar()
    for _=2, l do
        r = r .. getRandomChar()
    end
    return r
end

local base_type_and_default = {


    ["bool"] = "false",
    ["int"] = "0",
    ["float"] = "0.0f",
    ["double"] = "0.0",
    ["string"] = "null"
}

local base_type_randomfunction =
{


    ["bool"] = function(a, b, c)
        local r = math.random(1, 5)
        if r == 1 then
            return {
    a .. " = " .. b .. " && " .. c ..";", }
        elseif r == 2 then
            return {
    
                "if(" .. a .. ") ",
                "{",
                "    ".. b.. " = !" .. c .. ";",
                "}",
            }
        elseif r == 3 then
            return {
    
                "if(" .. a .. " && " .. c .. ") ",
                "{",
                "    ".. b.. " = !" .. b .. ";",
                "}",
            }
        elseif r == 4 then
            return {
    
                "if(" .. a .. " || " .. b .. ") ",
                "{",
                "    ".. b.. " = !" .. b .. ";",
                "}",
            }
        elseif r == 5 then
            return {
    a .. " = " .. b .. " || " .. c ..";", }
        else
    return {
    a.." = "..b.." && "..c..";", }
end
end,

["int"] = function(a, b, c)
        local r = math.random(1, 7)
        if r == 1 then
            return {
    a.." = "..b.." + "..c..";", }
elseif r==2 then
            return {
    a .. " = " .. b .. " - " .. c ..";", }
        elseif r==3 then
            return {
    a .. " = " .. b .. " * " .. c ..";", }
        elseif r==4 then
            return {
    a .. " = " .. b .. " / " .. c ..";", }
        elseif r==5 then
            return {
    
                a .. " = " .. math.random(1, 100000) ..";",
                b.." = "..math.random(1, 100000)..";",
                c.." = "..math.random(1, 100000)..";",
            }
        elseif r==6 then
            return {
    
                "for(int i=0;i<"..a..";++i)",
                "{",
                "	"..b .."+=1;",
                "   "..c .."+=" .. b..";",
                "}",
            }
        else
    return {

    b.." = "..a..";",
                c.." = "..a..";",
            }
end
end,

["float"] = function(a, b, c)
        local r = math.random(1, 6)
        if r == 1 then
            return {
    a.." = "..b.." + "..c..";", }
elseif r==2 then
            return {
    a .. " = " .. b .. " - " .. c ..";", }
        elseif r==3 then
            return {
    a .. " = " .. b .. " * " .. c ..";", }
        elseif r==4 then
            return {
    a .. " = " .. b .. " / " .. c ..";", }
        elseif r==5 then
            return {
    
                a .. " = " .. math.random(1, 10000) ..".0f;",
                b.." = "..math.random(1, 10000)..".0f;",
                c.." = "..math.random(1, 10000)..".0f;",
            }
        else
    return {

    b.." = "..a..";",
                c.." = "..a..";",
            }
end
end,

["double"] = function(a, b, c)
        local r = math.random(1, 6)
        if r == 1 then
            return {
    a.." = "..b.." + "..c..";", }
elseif r==2 then
            return {
    a .. " = " .. b .. " - " .. c ..";", }
        elseif r==3 then
            return {
    a .. " = " .. b .. " * " .. c ..";", }
        elseif r==4 then
            return {
    a .. " = " .. b .. " / " .. c ..";", }
        elseif r==5 then
            return {
    
                a .. " = " .. math.random(1, 10000) ..".0;",
                b.." = "..math.random(1, 10000)..".0;",
                c.." = "..math.random(1, 10000)..".0;",
            }
        else
    return {

    b.." = "..a..";",
                c.." = "..a..";",
            }
end
end,

["string"] = function(a, b, c)
        local r = math.random(1, 3)
        if r == 1 then
            return {
    a.." = "..b.." + "..c..";", }
elseif r==2 then
            return {
    
                a .. " = string.Format("..c ..","..b..");",
            }
        else
    return {

    b.." = "..a..";",
                c.." = "..a..";",
            }
end
end,
}

local base_type_list = nil
local function getRandomType()
    if base_type_list == nil then
        base_type_list = {
    }
        for k, _ in pairs(base_type_and_default) do
        table.insert(base_type_list, k)
        end
    end
    local r = math.random(1, #base_type_list)
    local t = base_type_list[r]
return t, base_type_and_default[t]
end
local function getRandomPublic()
    local r = math.random(1, 3)
    if r==1 then
        return "public"
    elseif r==2 then
        return "private"
    else
        return ""
    end
end

local property_info =
{
    
    p = "",
    name = "default",
    t = "int",
    v = "0",
}
function PropertyGenerate(p, n, t, v)
    local ta = setmetatable({
    },
        {

    __index = property_info,
        })

    ta.p = p or ta.p
    ta.name = n
    ta.t = t
    ta.v = v
    return ta
end

local attribute_info =
{

    p = "",
    name = "default",
    t = "int",
    property_info = nil,
}
function AttributeGenerate(p, n, t, pi)
    local ta = setmetatable({
    },
        {

    __index = attribute_info,
        })

    ta.p = p or ta.p
    ta.name = n
    ta.property_info = pi
    if pi~=nil then
        ta.t = pi.t
    end
    return ta
end

local method_info =
{

    p = "",

    name = "default",
    --[[
        {
        { "type1", "name1"}, { "type2", "name2"}, ..}
    ]]
    params = nil,
    --[[
        { "type", "value"}
    ]]
    retn = nil,

    content = nil,
}
function MethodContentGenerate(ci)

    content = {
}

local function getRandomP(t)
        return t[math.random(1, #t)]
    end

    for i = 1, 10 do
        local t = getRandomType()
  
          local m = ci.typemap[t]
        local f = base_type_randomfunction[t]
        if #m > 0 then
            local a = getRandomP(m)
            local c = f(a, getRandomP(m), getRandomP(m))
            for _, v in ipairs(c) do
    table.insert(content, v)
            end
        end
    end

    return content
end

function MethodGenerate(ci, p, n, r)
    local ta = setmetatable({
    },
        {

    __index = method_info,
        })

    ta.p = p or ta.p
    ta.name = n
    ta.retn = r
    ta.content = MethodContentGenerate(ci)
    return ta
end

local class_info =
{

    p = "",
    name = "default",
    implement = nil,

    properties = nil,
    attributes = nil,

    typemap = nil,
}

local classes = {
    }

local classnames = {
    }
function GetNextClassName(min, max)
    local r = math.random(min or 10, max or 10)
    while true do
        local n = getRandomName(r)
        if classnames[n] == nil then
            classnames[n] = true
            return n
        end
    end
end

--[[
-  Class generating function 
-  Parameters

    mc:  Number of functions

    pc:  Number of variables

    ac:  Attribute quantity
]]
function ClassGenerater(mc, pc, ac)
    local class = setmetatable({
},
        {

    __index = class_info,
        })

    local usedname = {
    }
    function GetNextName(min, max)
        local r = math.random(min or 10, max or 10)
        while true do
            local n = getRandomName(r)
            if usedname[n] == nil then
                usedname[n] = true
                return n
            end
        end
    end

    class.name = GetNextClassName(10, 40)
    usedname[class.name] = true;
--class.implement = "MonoBehaviour"

    class.properties = {
}
for i = 1, pc or 20 do
        table.insert(class.properties,
            PropertyGenerate(
                getRandomPublic(),
                GetNextName(10, 40),
                getRandomType()
            ))
    end

    class.attributes = {
}
for i = 1, ac or 20 do
        local t, v = getRandomType()

      local pi = nil
        if math.random(10) < 3 then
            pi = class.properties[math.random(#class.properties)]
        end
        table.insert(class.attributes,
            AttributeGenerate(
                "public",
                GetNextName(10, 40),
                t, pi
            ))
    end

    class.typemap = {
}
for _, v in ipairs(base_type_list) do
        class.typemap[v] = {
}
end
    for _, v in ipairs(class.properties) do
    table.insert(class.typemap[v.t], v.name)
    end
    for _, v in ipairs(class.attributes) do
    table.insert(class.typemap[v.t], v.name)
    end

    class.methods = {
}
for i = 1, mc or 20 do
        local t, v = getRandomType()

      local pi = nil
        if math.random(10) < 3 then
            pi = class.properties[math.random(#class.properties)]
        end
        table.insert(class.methods,
            MethodGenerate(class,
                "public",
                GetNextName(10, 40)
            ))
    end

    return class
end

using System;  
using System.Text;  
  
namespace ConsoleApp7  
{  
  class RandomNumberSample  
  {  
    static void Main(string[] args)  
    {  
      var generator = new RandomGenerator();  
      var randomNumber = generator.RandomNumber(5, 100);  
      Console.WriteLine($"Random number between 5 and 100 is {randomNumber}");  
  
      var randomString = generator.RandomString(10);  
      Console.WriteLine($"Random string of 10 chars is {randomString}");  
  
      var randomPassword = generator.RandomPassword();  
      Console.WriteLine($"Random string of 6 chars is {randomPassword}");  
  
      Console.ReadKey();  
    }  
  }  
  
  public class RandomGenerator  
  {  
    // Instantiate random number generator.  
    // It is better to keep a single Random instance 
    // and keep using Next on the same instance.  
    private readonly Random _random = new Random();  
  
    // Generates a random number within a range.      
    public int RandomNumber(int min, int max)  
    {  
      return _random.Next(min, max);  
    }  
  
    // Generates a random string with a given size.    
    public string RandomString(int size, bool lowerCase = false)  
    {  
      var builder = new StringBuilder(size);  
  
      // Unicode/ASCII Letters are divided into two blocks
      // (Letters 65–90 / 97–122):   
      // The first group containing the uppercase letters and
      // the second group containing the lowercase.  

      // char is a single Unicode character  
      char offset = lowerCase ? 'a' : 'A';  
      const int lettersOffset = 26; // A...Z or a..z: length = 26  
  
      for (var i = 0; i < size; i++)  
      {  
        var @char = (char)_random.Next(offset, offset + lettersOffset);  
        builder.Append(@char);  
      }  
  
      return lowerCase ? builder.ToString().ToLower() : builder.ToString();  
    }  
  
    // Generates a random password.  
    // 4-LowerCase + 4-Digits + 2-UpperCase  
    public string RandomPassword()  
    {  
      var passwordBuilder = new StringBuilder();  
  
      // 4-Letters lower case   
      passwordBuilder.Append(RandomString(4, true));  
  
      // 4-Digits between 1000 and 9999  
      passwordBuilder.Append(RandomNumber(1000, 9999));  
  
      // 2-Letters upper case  
      passwordBuilder.Append(RandomString(2));  
      return passwordBuilder.ToString();  
    }  
  }  
}  
*/