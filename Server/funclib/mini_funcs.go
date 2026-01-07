package funclib

import (
	"crypto/sha256"
	"fmt"
	"log"
	"time"

	// Libs
	"udlib/data"

	// Database
	"database/sql"
	_ "github.com/go-sql-driver/mysql"
)

// Alias for TokenChecker output values
type tokenGetter func(string, []byte)([]byte, error)
// Generates token for a current session
func GetToken(user string, hs_psw []byte) (token []byte, null error) {

  // Logging
  if user == "" || hs_psw == nil{
    log.Printf("GetToken: Logging | Some NULL's")
    return nil, fmt.Errorf("NULL")
  }

  // Generates Token
  h := sha256.New()
  t := time.Now().Add(10 * time.Minute)
  h.Write([]byte(user + string(hs_psw) + t.GoString() + data.Salt))
  token = h.Sum(nil)
  return token, nil
}


// Toker Checker 
func CheckToken(user string, token []byte, db *sql.DB) (gen_tk tokenGetter, exist bool) {

  t_cols := data.TokenCols
  u_cols := data.UserCols

  querry := fmt.Sprintf(`
    SELECT t.%s FROM %s AS t
    JOIN %s AS u ON t.%s=u.%s
    WHERE t.%s=? AND t.%s=?
  `,
  t_cols.Time, data.Token_Table,
  data.User_Table, t_cols.UserName, u_cols.Login,
  t_cols.UserName, t_cols.Token)

  // NOT FINISHED
  var tokenTime string
  if err := db.QueryRow(querry, user, token).Scan(&tokenTime); err != nil {
    log.Printf("TokenChecker: DB Query | Error ==> %s", err)
    return GetToken, false
  }
  return nil, true
}

// Hash Password Generator
func CreateHashPsw(psw string) []byte {
  h := sha256.New()
  h.Write([]byte(psw + data.Salt))
  hash := h.Sum(nil)
  return hash
}


func UserCheck(user string, db *sql.DB) (exist bool, err error) {

  u_cols := data.UserCols
  query := fmt.Sprintf(`
    SELECT u.%s FROM %s
    WHERE u.%s=?
  `, u_cols.Login, data.User_Table, u_cols.Login)
  
  var name string
  if err := db.QueryRow(query, user).Scan(&name); err == sql.ErrNoRows {
    return false, nil
  } else if err != nil {
    return false, fmt.Errorf("%s", err)
  }
  return true, nil

}
