package data


// Setting variables
var DB_Dir string = "ignored/db_guard.json"





// Global variables
var Login string = "login"
var Password string = "password"
var Groupid string = "groupid"
var Id string = "id"





// Structs
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





// Defining user_t table for convinience 
type user_t struct {
  ID, Login, Password, IsAdmin, GroupID string
}
var User_t = user_t{ID: "ID", Login: "Login", Password: "Password", IsAdmin: "IsAdmin", GroupID: "GroupID"}
const User_Table = "user_t"
