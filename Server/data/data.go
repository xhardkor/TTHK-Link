package data

// Global vars
var Login string = "login"
var Password string = "password"
var Groupid string = "groupid"
var Id string = "id"


type DBConfig struct {
  DBUser string `json:"db_user"`
  DBPass string `json:"db_pass"`
  DBHost string `json:"db_host"`
  DBPort int    `json:"db_port"`
  DBName string `json:"db_name"`
}

type UserJSON struct {
  ID        int     `json:"id,omitempty"`
  User      string  `json:"user,omitempty"`
  Password  string  `json:"password,omitempty"`
  IsAdmin   bool    `json:"is_admin,omitempty"`
  GroupID   string  `json:"group_id,omitempty"`
}

