package funclib

import (
	"fmt"
	"net/http"
	"udlib/data"

	"database/sql"
	_ "github.com/go-sql-driver/mysql"
)

func ParseRequest(r *http.Request) data.RequestData {
  return data.RequestData{
  }
}


// Func for DB id checking
func DB_Checking(db *sql.DB, sele, from, whr, imp_id string) (status bool) {
  querry := fmt.Sprintf("SELECT %s FROM %s WHERE %s=?", sele, from, whr)
  id := db.QueryRow(querry, imp_id)
  if id != nil {
    return false
  }
  return true
}
