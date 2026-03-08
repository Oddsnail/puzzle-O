using System;
using System.Collections.Generic;
using UnityEngine;

namespace origin.command {
    public class CommandDatabase {
        private Dictionary<string, Delegate> database = new();

        public bool HasCommand(string commandName) {
            if (string.IsNullOrWhiteSpace(commandName)) return false;
            return database.ContainsKey(commandName.ToLower());
        }

        public void AddCommand(string commandName, Delegate command) {
            if (string.IsNullOrWhiteSpace(commandName)) {
                Debug.LogError("[ERROR] Command name cannot be null or empty");
                return;
            }

            string normalizedName = commandName.ToLower();

            if (database.ContainsKey(normalizedName)) {
                Debug.LogError($"[ERROR] Command '{commandName}' already exists in the database. Skipping.");
                return;
            }

            if (command == null) {
                Debug.LogError($"[ERROR] Cannot add null delegate for command '{commandName}'");
                return;
            }

            database.Add(normalizedName, command);
        }

        public Delegate GetCommand(string commandName) {
            if (string.IsNullOrWhiteSpace(commandName)) return null;

            string normalizedName = commandName.ToLower();

            if (!database.ContainsKey(normalizedName)) {
                Debug.LogError($"[ERROR] Command '{commandName}' does not exist in the database");
                return null;
            }
            return database[normalizedName];
        }
    }
}