package hu.bme.aut.android.todoapp.data

import androidx.room.Dao
import androidx.room.Delete
import androidx.room.Insert
import androidx.room.Query
import androidx.room.Update

@Dao
interface TodoItemDao {
    @Query("SELECT * FROM todoitem")
    fun getAll(): List<TodoItem>

    @Query("SELECT * FROM todoitem WHERE completed = :completed")
    fun getItemByIsCompleted(completed: Boolean): List<TodoItem>

    @Insert
    fun insert(todoItem: TodoItem): Long

    @Update
    fun update(todoItem: TodoItem)

    @Delete
    fun deleteItem(todoItem: TodoItem)
}