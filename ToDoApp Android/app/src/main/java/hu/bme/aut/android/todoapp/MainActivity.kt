package hu.bme.aut.android.todoapp

import android.content.Intent
import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import hu.bme.aut.android.todoapp.adapter.TodoAdapter
import hu.bme.aut.android.todoapp.data.TodoItemDatabase
import hu.bme.aut.android.todoapp.data.TodoItem
import hu.bme.aut.android.todoapp.databinding.ActivityMainBinding
import hu.bme.aut.android.todoapp.fragment.EditTodoItemDialogFragment
import hu.bme.aut.android.todoapp.fragment.ViewTodoItemDialogFragment
import hu.bme.aut.android.todoapp.fragment.NewTodoItemDialogFragment

import kotlin.concurrent.thread

class MainActivity : AppCompatActivity(), TodoAdapter.TodoItemClickListener,
    NewTodoItemDialogFragment.NewTodoItemDialogListener, ViewTodoItemDialogFragment.ViewTodoItemDialogListener,
    EditTodoItemDialogFragment.EditTodoItemDialogListener {

    private lateinit var binding: ActivityMainBinding
    private lateinit var database: TodoItemDatabase
    private lateinit var adapter: TodoAdapter

    private lateinit var todoItem: TodoItem


    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(binding.root)

        setSupportActionBar(binding.toolbar)

        database = TodoItemDatabase.getDatabase(applicationContext)

        binding.fabNewItem.setOnClickListener {
            NewTodoItemDialogFragment().show(
                supportFragmentManager,
                NewTodoItemDialogFragment.TAG
            )
        }

        binding.chartHoliday.setOnClickListener {
            startActivity(Intent(this, ChartActivity::class.java))
        }

        initRecyclerView()
    }

    private fun initRecyclerView() {
        adapter = TodoAdapter(this)
        binding.rvMain.layoutManager = LinearLayoutManager(this)
        binding.rvMain.adapter = adapter
        loadItemsInBackground()
    }

    private fun loadItemsInBackground() {
        thread {
            val items = database.todoItemDao().getAll()
            runOnUiThread {
                adapter.update(items)
            }
        }
    }

    override fun onItemChanged(item: TodoItem) {
        thread {
            database.todoItemDao().deleteItem(item)
            runOnUiThread {
                adapter.removeItem(item)
            }
            loadItemsInBackground()
        }
    }

    override fun onTodoItemCreated(newItem: TodoItem) {
        thread {
            val insertId = database.todoItemDao().insert(newItem)
            newItem.id = insertId
            runOnUiThread {
                adapter.addItem(newItem)
            }
        }
    }

    override fun getTodoItemDetails(): TodoItem {
        return todoItem
    }

    override fun getAllItems(): List<TodoItem> {
        return getAllItems()
    }

    override fun onItemEdited(item: TodoItem) {
        todoItem = item
        onItemChanged(item)
        EditTodoItemDialogFragment().show(
            supportFragmentManager,
            ViewTodoItemDialogFragment.TAG
        )
    }

    override fun onItemClicked(item: TodoItem) {
        todoItem = item
        ViewTodoItemDialogFragment().show(
            supportFragmentManager,
            ViewTodoItemDialogFragment.TAG
        )
    }
}
