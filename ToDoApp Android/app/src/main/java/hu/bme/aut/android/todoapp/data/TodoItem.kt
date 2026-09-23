package hu.bme.aut.android.todoapp.data

import androidx.room.ColumnInfo
import androidx.room.Entity
import androidx.room.PrimaryKey
import androidx.room.TypeConverter

@Entity(tableName = "todoitem")
data class TodoItem(
    @ColumnInfo(name = "id") @PrimaryKey(autoGenerate = true) var id: Long? = null,
    @ColumnInfo(name = "title") var title: String,
    @ColumnInfo(name = "description") var description: String,
    @ColumnInfo(name = "date") var date: String,
    @ColumnInfo(name = "time") var time: String,
    @ColumnInfo(name = "completed") var completed: Boolean,
    @ColumnInfo(name = "priority") var priority: Priority
    ) : java.io.Serializable {
        enum class Priority {
            HIGH, LOW;

            companion object {
                @JvmStatic
                @TypeConverter
                fun getByOrdinal(ordinal: Int): Priority? {
                    var ret: Priority? = null
                    for (cat in values()) {
                        if (cat.ordinal == ordinal) {
                            ret = cat
                            break
                        }
                    }
                    return ret
                }
                @JvmStatic
                @TypeConverter
                fun toInt(category: Priority): Int {
                    return category.ordinal
                }
            }
        }
    }