package hu.bme.aut.android.todoapp.data

import android.content.Context
import androidx.room.Database
import androidx.room.Room
import androidx.room.RoomDatabase
import androidx.room.TypeConverters

@Database(entities = [TodoItem::class], version = 1)
@TypeConverters(value = [TodoItem.Priority::class])
abstract class TodoItemDatabase : RoomDatabase() {
    abstract fun todoItemDao(): TodoItemDao

    companion object {
        fun getDatabase(applicationContext: Context): TodoItemDatabase {
            return Room.databaseBuilder(
                applicationContext,
                TodoItemDatabase::class.java,
                "todo-list"
            ).build()
        }
    }
}