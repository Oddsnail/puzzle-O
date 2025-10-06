using System;
using System.Collections.Generic;
using UnityEngine;

public class CommandDatabase
{
    private Dictionary<string, Delegate> database = new();

    public bool HasCommand(string commandName) => database.ContainsKey(commandName);

    public void AddCommand(string commandName, Delegate command) {

        if (database.ContainsKey(commandName)) Debug.LogError($"Command {commandName} already exists in the database");
        database.Add(commandName, command);
    }

    public Delegate GetCommand(string commandName) {

        if (!database.ContainsKey(commandName)) {
            Debug.LogError($"Command {commandName} do not exist in the database");
            return null;
        }
        return database[commandName];
    }
}
