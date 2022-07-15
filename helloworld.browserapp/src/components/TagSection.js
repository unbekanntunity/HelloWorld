import React, { Component } from 'react';

import DropRightDialog from './DropRightDialog';
import InputField from './InputField/InputField';
import Tag from './Tag';

import remove from '../images/delete.png';
import add from '../images/add-circle.png';
import close from '../images/close-circle.jpg';

import { sendJSONRequest } from '../requestFuncs';

import './TagSection.css';

class TagSection extends Component {
    state = {
        showAddTagDialog: false,
        allTags: [],
        filteredTags: [],
        addedTags: [],
        currentInput: "",
    }

    componentDidMount() {
        setInterval(() => this.getTagsPerRequest(), 1000);
    }

    getTags = () => {
        return this.state.addedTags.map(item => item.name);
    }

    updateAddedTags = (tags) => {
        this.setState({ addedTags: tags });
    }

    getTagsPerRequest = async () => {
        await sendJSONRequest("GET", "/tag/get_all", undefined, this.props.tokens.token)
            .then(response => {
                this.setState({
                    allTags: response.data,
                    filteredTags: this.filterTags(response.data)
                })
            }, error => {
                this.props.onError(error.message);
            }
        );
    }

    filterTags = (allData) => {
        return allData.filter(item =>
            item.name.indexOf(this.state.currentInput) !== -1 && this.state.addedTags.map(x => x.name).indexOf(item.name) === -1)
    }

    handleInput = (event) => {
        this.setState({
            currentInput: event.target.value,
            filteredTags: this.filterTags(this.state.allTags)
        });
    }

    handleTagSelected = (index) => {
        let selectedItem = this.state.filteredTags[index];

        let oldArray = this.state.addedTags;
        let newArray = [...oldArray, selectedItem];

        this.setState({
            addedTags: newArray,
            filteredTags: this.state.filteredTags.filter(x => x.name != selectedItem.name)
        });
    }

    handleDropRightToggle = () => {
        this.setState({
            showAddTagDialog: !this.state.showAddTagDialog
        })
    }

    handleRemove = (index) => {
        const filteredArray = [...this.state.filteredTags, this.state.addedTags[index]];
        this.setState({
            filteredTags: filteredArray,
            addedTags: this.state.addedTags.filter((item, addedIndex) => addedIndex !== index),
        })
    }

    renderTags = () => {
        if (this.state.filteredTags.length === 0) {
            return <InputField.CenterLineItem header="No items found" clickable={false} />
        }
        else {
            return this.state.filteredTags.map((item, index) => {
                let text = this.getTagsCategory(item);
                return <InputField.TwoLineItem key={index} header={item.name} clickable={true}
                    text={text} />
                }
            )
        }
    }

    getTagsCategory = (item) => {
        switch (this.props.tagType) {
            case "Posts":
                return `${item.postsTaged} posts`;
            case "Discussions":
                return `${item.discussionsTaged} discussions`;
            case "Projects":
                return `${item.projectsTaged} projects`;
            case "Users":
                return `${item.usersTaged} users`;
            default:
                return "No type supplied";
        }
    }

    render() {
        return (
            <div>
                <div className="section-header">
                    <p className="section-header-text" style={{
                        fontSize: this.props.headerSize ?? ""
                    }}>Tags</p>
                    <div className="tagSection-headerDrop">
                        <DropRightDialog menuOpenedIcon={close} menuClosedIcon={add} iconSize={20}
                            onToggleButtonClick={this.handleDropRightToggle} show={this.state.showAddTagDialog} zIndex={this.props.zIndex}>
                            <div className="tagSection-headerdrop-container">
                                <InputField design='m2' name="tagSearch" onItemClick={this.handleTagSelected} onChange={this.handleInput} showUnderline={true}>
                                {
                                    this.renderTags()
                                }
                                </InputField>
                            </div>
                        </DropRightDialog>
                    </div>
                </div>
                <div className="flex">
                {
                    this.state.addedTags &&
                    this.state.addedTags.map((item, index) =>
                        <Tag key={index} name={item.name ?? item} propKey={index} margin="0px 10px 0px 0px"
                            removable={true} removeIcon={remove} iconSize={20} onRemove={this.handleRemove} /> )
                }
                </div>
            </div>
        );
    }
}

export default TagSection;